namespace DataSource;

internal class DataSourceCache
{
    private Dictionary<string, DataSourceCacheContainer> _cache = new();

    private static DataSourceCache? _instance;

    public static DataSourceCache Instance { get; private set; } = _instance ??= new DataSourceCache();

    public async Task<DataQueryResult> Get(string id, DataSourceCacheStamp dataSourceCacheStamp, Func<Task<DataQueryResult>> factory)
    {
        if (dataSourceCacheStamp.CacheIsExpired(_cache))
            await StoreContainer(id, factory);

        dataSourceCacheStamp.ResetFromCache(_cache);
        return _cache[id].Result;
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
