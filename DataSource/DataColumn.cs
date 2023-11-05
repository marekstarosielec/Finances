namespace DataSource;

public class DataColumn
{
    public string ColumnName { get; }

    public DataType ColumnDataType { get; }

    public DataColumn(string columnName, DataType columnDataType)
    {
        ColumnName = columnName;
        ColumnDataType = columnDataType;
    }
}
