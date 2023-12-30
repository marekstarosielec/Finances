namespace DataSource;

public interface IDataSource
{
    Dictionary<string, DataColumn> Columns { get; }
    Task<DataQueryResult> ExecuteQuery(DataQuery? dataQuery = null);
    void RemoveCache();
    DateTime? CacheTimeStamp { get; }

    Task Save(DataRow row);
}