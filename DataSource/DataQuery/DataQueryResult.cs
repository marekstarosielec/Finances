namespace DataSource;

public class DataQueryResult
{
    public IEnumerable<DataColumn> Columns { get; }
    public IEnumerable<DataRow> Rows { get; }
    public int TotalRowCount { get; }

    public DataQueryResult(IEnumerable<DataColumn> columns, IEnumerable<DataRow> rows, int totalRowCount)
    {
        Columns = columns;
        Rows = rows;
        TotalRowCount = totalRowCount;
    }

    public DataRow? GetById(string id) => Rows.FirstOrDefault(r => r.Id?.OriginalValue as string == id);

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
