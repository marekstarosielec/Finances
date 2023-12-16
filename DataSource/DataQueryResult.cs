namespace DataSource;

public class DataQueryResult
{
    public IEnumerable<DataColumn> Columns { get; }
    public IEnumerable<Dictionary<DataColumn, object?>> Rows { get; }
    public int TotalRowCount { get; }

    private DataColumn? IdColumn;

    public DataQueryResult(IEnumerable<DataColumn> columns, IEnumerable<Dictionary<DataColumn, object?>> rows, int totalRowCount)
    {
        Columns = columns;
        Rows = rows;
        TotalRowCount = totalRowCount;
        IdColumn = Columns.FirstOrDefault(c => c.ColumnName == "Id");
    }

    public Dictionary<DataColumn, object?>? GetById(string id)
    {
        if (IdColumn == null)
            throw new InvalidOperationException("Column Id not present in data source");

        return Rows.FirstOrDefault(r => r[IdColumn]?.ToString() == id);
    }
}
