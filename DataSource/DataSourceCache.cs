namespace DataSource;

public class DataSourceCache<T>
{
    private T? _cache;

    public DateTime TimeStamp { get; private set; } = DateTime.MinValue;

    public async Task<T> Get(Func<Task<T>> getData)
    {
        if (_cache != null)
            return _cache;
        
        _cache = await getData();
        TimeStamp = DateTime.UtcNow;
        return _cache;
    }

    public void Clean()
    {
        _cache = default;
        TimeStamp = DateTime.MinValue;
    }
}
