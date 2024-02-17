namespace DataSource;

internal class DataSourceCache
{
    
    private static DataSourceCache? _instance;

    public static DataSourceCache Instance { get; private set; } = _instance ??= new DataSourceCache();

    private Dictionary<string, DataSourceCacheContainer> _cache = new();
    private bool _areDependenciesListBuilt;
    
    public DataSourceCacheStamp Register(string id, Func<Task<DataQueryResult>> factory, params string[] relatedIds)
    {
        _cache[id] = new DataSourceCacheContainer
        {
            TimeStamp = DateTime.MinValue,
            Factory = factory,
            RelatedIds = relatedIds.ToList()
        };
        var allIds = relatedIds.ToList();
        allIds.Add(id);
        return new DataSourceCacheStamp(allIds);
    }

    public async Task<DataQueryResult> Get(string id, DataSourceCacheStamp dataSourceCacheStamp)
    {
        if (!_areDependenciesListBuilt)
            BuildDependenciesLists();

        if (dataSourceCacheStamp.CacheIsExpired(_cache))
            await StoreContainer(id, _cache[id].Factory);

        //Add new related ids, if they were added during BuildDependenciesLists.
        foreach (string relatedId in _cache[id].RelatedIds)
            dataSourceCacheStamp.AddNewRelatedId(relatedId);

        dataSourceCacheStamp.ResetFromCache(_cache);
        return _cache[id].Result!;
    }

    public void Clean(string id)
    {
        _cache[id].TimeStamp = DateTime.MinValue;
        _cache[id].Result = null;
    }

    private void BuildDependenciesLists()
    {
        var source = new Dictionary<string, List<string>>();
        foreach (var id in _cache.Keys)
            source[id] = new List<string>(_cache[id].RelatedIds);
        var relationsFinder = new DataSourceRelationsFinder(source);
        foreach (var id in _cache.Keys)
            _cache[id].RelatedIds=relationsFinder.FindAllRelatedElements(id).ToList();

        _areDependenciesListBuilt = true;
    }

    private async Task StoreContainer(string id, Func<Task<DataQueryResult>> factory)
    {
        _cache[id].Result = await factory.Invoke();
        _cache[id].TimeStamp = DateTime.UtcNow;
    }
}
