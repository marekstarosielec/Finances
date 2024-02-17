namespace DataSource;

public interface IDataSource
{
    string Id { get; }

    Dictionary<string, DataColumn> Columns { get; }

    Task<DataQueryResult> ExecuteQuery(DataQuery dataQuery);

    void RemoveCache();

    Task Save(List<DataRow> rows);
}