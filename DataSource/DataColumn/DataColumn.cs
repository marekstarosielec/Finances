namespace DataSource;

public class DataColumn
{
    public string ColumnName { get; }

    public ColumnDataType ColumnDataType { get; }

    public DataColumn(string columnName, ColumnDataType columnDataType)
    {
        ColumnName = columnName;
        ColumnDataType = columnDataType;
    }
}
