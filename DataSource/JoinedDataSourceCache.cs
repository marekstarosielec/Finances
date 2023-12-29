namespace DataSource;

public class JoinedDataSourceCache
{
    private IEnumerable<DataRow>? _cache;

    private DateTime? _leftDataSourceCacheTimeStamp;
    private DateTime? _rightDataSourceCacheTimeStamp;
    public DateTime TimeStamp { get; private set; } = DateTime.MinValue;

    public async Task<IEnumerable<DataRow>> Get(IDataSource leftDataSource, IDataSource rightDataSource, Func<Task<IEnumerable<DataRow>>> getData)
    {
        if (leftDataSource.CacheTimeStamp == _leftDataSourceCacheTimeStamp 
            && rightDataSource.CacheTimeStamp == _rightDataSourceCacheTimeStamp 
            && _cache != null)
            return _cache;

        _cache = await getData();
        TimeStamp = DateTime.UtcNow;
        _leftDataSourceCacheTimeStamp = leftDataSource.CacheTimeStamp; 
        _rightDataSourceCacheTimeStamp = rightDataSource.CacheTimeStamp;
        return _cache;
    }

    public void Clean()
    {
        _cache = default;
        TimeStamp = DateTime.MinValue;
        _leftDataSourceCacheTimeStamp = null;
        _rightDataSourceCacheTimeStamp = null;
    }
}
