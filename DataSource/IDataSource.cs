namespace DataSource;

public interface IDataSource
{
    Dictionary<string, DataColumn> Columns { get; }
    Task<DataView> GetDataView(DataQuery? dataQuery = null);
}