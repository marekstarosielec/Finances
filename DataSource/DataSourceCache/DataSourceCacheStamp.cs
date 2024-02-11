namespace DataSource;

internal class DataSourceCacheStamp
{
    private Dictionary<string, DateTime> _stamps = new Dictionary<string, DateTime>();

    public DataSourceCacheStamp(params string[] ids)
    {
        foreach (var id in ids)
            _stamps[id] = DateTime.MinValue;
    }

    /// <summary>
    /// Sets time stamps in _stamps to ones provided in cache.
    /// </summary>
    /// <param name="cache"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void ResetFromCache(Dictionary<string, DataSourceCacheContainer> cache)
    {
        foreach (var id in _stamps.Keys)
        {
            //Cache does not contain requested data.
            if (!cache.TryGetValue(id, out var container))
                throw new InvalidOperationException("Cannot set stamp on non existing cache");

            //Cached data timestamp is different then previously used timestamp. 
            _stamps[id] = container.TimeStamp;
        }
    }


    /// <summary>
    /// Checks if _stamps are different fromn ones provided in cache.
    /// </summary>
    /// <param name="cache"></param>
    /// <returns></returns>
    public bool CacheIsExpired(Dictionary<string, DataSourceCacheContainer> cache)
    {
        foreach(var id in _stamps.Keys)
        {
            //Cache does not contain requested data.
            if (!cache.TryGetValue(id, out var container))
                return true;

            //Cached data timestamp is different then previously used timestamp. 
            if (_stamps[id] != container.TimeStamp)
                return true;
        }
        return false;
    }
}
