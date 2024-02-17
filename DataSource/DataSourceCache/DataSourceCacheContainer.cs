namespace DataSource;

/// <summary>
/// Contains cached data and creation timestamp. There is also a method for filling data.
/// </summary>
internal class DataSourceCacheContainer
{
    public DateTime TimeStamp { get; set; }
    public DataQueryResult? Result { get; set; }
    public Func<Task<DataQueryResult>> Factory { get; set; }
    public List<string> RelatedIds { get; set; }
}
