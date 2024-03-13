using System.Threading;

namespace DataSource;

internal class DataSourceCache
{
    
    private static DataSourceCache? _instance;

    public static DataSourceCache Instance { get; private set; } = _instance ??= new DataSourceCache();

    private Dictionary<string, DataSourceCacheContainer> _cache = new();
    
    private bool _areDependenciesListBuilt;

    private static Dictionary<string, SemaphoreSlim> _semaphores = new();


    public void Register(string id, Func<Task<DataQueryResult>> factory, params string[] relatedIds)
    {
        _cache[id] = new DataSourceCacheContainer
        {
            TimeStamp = DateTime.MinValue,
            Factory = factory,
            RelatedIds = relatedIds.ToList()
        };
        var allIds = relatedIds.ToList();
        allIds.Add(id);
    }

    /// <summary>
    /// Get data from cache, or if not available or expired, from data source.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<DataQueryResult> Get(string id)
    {
        var _semaphore = GetSemaphore(id);
        try
        {
            _semaphore.Wait();
            if (!_areDependenciesListBuilt)
                BuildDependenciesLists();

            if (!_cache.TryGetValue(id, out var container) || container.TimeStamp == DateTime.MinValue)
                await RefreshCache(id, _cache[id].Factory);

            return _cache[id].Result!;
        }
        catch (Exception e)
        {
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Remove data from cache. All the related data is removed too, as it could have been modified (e.g. in union data source).
    /// </summary>
    /// <param name="id"></param>
    public void Clean(string id)
    {
        foreach (var relatedId in _cache[id].RelatedIds)
        {
            _cache[relatedId].TimeStamp = DateTime.MinValue;
            _cache[relatedId].Result = null;
            _cache[relatedId].Invalidated = true;
        }
        _cache[id].TimeStamp = DateTime.MinValue;
        _cache[id].Result = null;
        _cache[id].Invalidated = true;
    }

    public bool IsCacheInvalidated(string id) => _cache.TryGetValue(id, out var container) && container.Invalidated;

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
        _cache[id].Invalidated = false;
    }

    private SemaphoreSlim GetSemaphore(string id)
    {
        if (!_semaphores.ContainsKey(id))
            _semaphores[id] = new SemaphoreSlim(1);
        return _semaphores[id];
    }
}
