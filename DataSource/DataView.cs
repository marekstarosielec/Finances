namespace DataSource;

public class DataView
{
    public IEnumerable<DataColumn> Columns { get; }
    public IEnumerable<Dictionary<DataColumn, object?>> Values { get; }
    public int TotalRows { get; }

    public DataView(IEnumerable<DataColumn> columns, IEnumerable<Dictionary<DataColumn, object?>> values, int totalRows)
    {
        Columns = columns;
        Values = values;
        TotalRows = totalRows;
    }
}
