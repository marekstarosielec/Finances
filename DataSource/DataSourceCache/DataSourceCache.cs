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

    /// <summary>
    /// Get data from cache, or if not available or expired, from data source.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dataSourceCacheStamp"></param>
    /// <returns></returns>
    public async Task<DataQueryResult> Get(string id, DataSourceCacheStamp dataSourceCacheStamp)
    {
        if (!_areDependenciesListBuilt)
            BuildDependenciesLists();

        //Add new related ids, if they were added during BuildDependenciesLists.
        foreach (string relatedId in _cache[id].RelatedIds)
            dataSourceCacheStamp.AddNewRelatedId(relatedId);

        //Make sure that cache contains all the data. Also contains main cache.
        foreach (var relatedId in dataSourceCacheStamp.Stamps.Keys)
            if (!_cache.TryGetValue(id, out var container) || container.TimeStamp == DateTime.MinValue)
                await RefreshCache(relatedId, _cache[relatedId].Factory);
      
        dataSourceCacheStamp.SetTimeStampsFromCache(_cache);
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

    private async Task RefreshCache(string id, Func<Task<DataQueryResult>> factory)
    {
        _cache[id].Result = await factory.Invoke();
        _cache[id].TimeStamp = DateTime.UtcNow;
    }
}
