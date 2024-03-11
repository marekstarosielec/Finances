using System.Collections.ObjectModel;

namespace DataSource;

internal class DataSourceCacheStamp
{
    private Dictionary<string, DateTime> _stamps = new();

    public ReadOnlyDictionary<string, DateTime> Stamps { get; init; }

    public DataSourceCacheStamp(List<string> ids)
    {
        foreach (var id in ids)
            _stamps[id] = DateTime.MinValue;

        Stamps = new ReadOnlyDictionary<string, DateTime>(_stamps);
    }

    public void AddNewRelatedId(string id)
    {
        if (!_stamps.ContainsKey(id))
            _stamps.Add(id, DateTime.MinValue);
    }

    /// <summary>
    /// Sets time stamps in _stamps to ones provided in cache.
    /// </summary>
    /// <param name="cache"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void SetTimeStampsFromCache(Dictionary<string, DataSourceCacheContainer> cache)
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
}
