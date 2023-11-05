namespace DataSource;

public interface IDataSource
{
    Task<DataView> GetDataView(DataQuery dataQuery);
}