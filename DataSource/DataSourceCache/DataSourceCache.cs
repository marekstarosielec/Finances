namespace DataSource;

internal class DataSourceCache
{
    private Dictionary<string, DataSourceCacheContainer> _cache = new();

    private static DataSourceCache? _instance;

    public static DataSourceCache Instance { get; private set; } = _instance ??= new DataSourceCache();

    private bool _areDependenciesListBuilt;

    public void Register(string id, Func<Task<DataQueryResult>> factory, params string[] relatedIds)
    {
        _cache[id] = new DataSourceCacheContainer
        {
            TimeStamp = DateTime.MinValue,
            Factory = factory,
            RelatedIds = relatedIds.ToList()
        };
    }

    public async Task<DataQueryResult> Get(string id, DataSourceCacheStamp dataSourceCacheStamp, Func<Task<DataQueryResult>> factory)
    {
        if (!_areDependenciesListBuilt)
            BuildDependenciesLists();

        if (dataSourceCacheStamp.CacheIsExpired(_cache))
            await StoreContainer(id, factory);

        dataSourceCacheStamp.ResetFromCache(_cache);
        return _cache[id].Result;
    }


    private void BuildDependenciesLists()
    {
        foreach (var id in _cache.Keys) 
            foreach (var id2 in _cache.Keys)
            {
                if (id == id2)
                    continue;

                var container = _cache[id2];
                if (container.RelatedIds.Contains(id)) 
                {
                    var parentContainer = _cache[id];
                    container.RelatedIds.AddRange(parentContainer.RelatedIds.Where(rid => rid != id2));
                    container.RelatedIds = container.RelatedIds.Distinct().ToList();
                }

            }
        
        _areDependenciesListBuilt = true;
    }

    private void FindAllRelatedIds(string id, HashSet<string> relatedIds)
    {
       
    }

    private async Task StoreContainer(string id, Func<Task<DataQueryResult>> factory)
    {
        var container = new DataSourceCacheContainer
        {
            Result = await factory.Invoke(),
            TimeStamp = DateTime.UtcNow
        };
        _cache.Add(id, container);
    }
}
