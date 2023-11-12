namespace DataSource;

public class DataQueryResult
{
    public IEnumerable<DataColumn> Columns { get; }
    public IEnumerable<Dictionary<DataColumn, object?>> Rows { get; }
    public int TotalRowCount { get; }

    public DataQueryResult(IEnumerable<DataColumn> columns, IEnumerable<Dictionary<DataColumn, object?>> rows, int totalRowCount)
    {
        Columns = columns;
        Rows = rows;
        TotalRowCount = totalRowCount;
    }
}
