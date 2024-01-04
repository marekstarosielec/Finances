namespace DataSource;

public class DataQueryResult
{
    public IEnumerable<DataColumn> Columns { get; }
    public IEnumerable<DataRow> Rows { get; }
    public int TotalRowCount { get; }

    private DataColumn? IdColumn;

    public DataQueryResult(IEnumerable<DataColumn> columns, IEnumerable<DataRow> rows, int totalRowCount)
    {
        Columns = columns;
        Rows = rows;
        TotalRowCount = totalRowCount;
        IdColumn = Columns.FirstOrDefault(c => c.ColumnName == "Id");
    }

    public DataRow? GetById(string id)
    {
        if (IdColumn == null)
            throw new InvalidOperationException("Column Id not present in data source");

        return Rows.FirstOrDefault(r => r[IdColumn]?.OriginalValue as string == id);
    }

    public DataQueryResult Clone()
    {
        var columns = new List<DataColumn>();
        foreach (var column in Columns)
            columns.Add(new DataColumn(column.ColumnName, column.ColumnDataType));

        var rows = new List<DataRow>();
        foreach (var row in Rows)
        { 
            var dataRow = new DataRow();    
            foreach (var key in row.Keys)
                dataRow[key] = new DataValue(row[key].OriginalValue);
            rows.Add(dataRow);
        }
        return new DataQueryResult(columns, rows, rows.Count);
    }
}
