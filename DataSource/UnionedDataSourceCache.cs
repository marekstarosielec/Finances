namespace DataSource;

public class UnionedDataSourceCache
{
    private IEnumerable<DataRow>? _cache;

    private DateTime? _firstDataSourceCacheTimeStamp;
    private DateTime? _secondDataSourceCacheTimeStamp;
    public DateTime TimeStamp { get; private set; } = DateTime.MinValue;

    public async Task<IEnumerable<DataRow>> Get(IDataSource firstDataSource, IDataSource secondDataSource, Func<Task<IEnumerable<DataRow>>> getData)
    {
        if (firstDataSource.CacheTimeStamp == _firstDataSourceCacheTimeStamp 
            && secondDataSource.CacheTimeStamp == _secondDataSourceCacheTimeStamp
            && _cache != null)
            return _cache;

        _cache = await getData();
        TimeStamp = DateTime.UtcNow;
        _firstDataSourceCacheTimeStamp = firstDataSource.CacheTimeStamp;
        _secondDataSourceCacheTimeStamp = secondDataSource.CacheTimeStamp;
        return _cache;
    }

    public void Clean()
    {
        _cache = default;
        TimeStamp = DateTime.MinValue;
        _firstDataSourceCacheTimeStamp = null;
        _secondDataSourceCacheTimeStamp = null;
    }
}
