namespace DataSource;

public class DataView
{
    public IEnumerable<DataColumn> Columns { get; }
    public IEnumerable<Dictionary<DataColumn, object?>> Rows { get; }
    public int TotalRowCount { get; }

    public DataView(IEnumerable<DataColumn> columns, IEnumerable<Dictionary<DataColumn, object?>> rows, int totalRowCount)
    {
        Columns = columns;
        Rows = rows;
        TotalRowCount = totalRowCount;
    }
}
