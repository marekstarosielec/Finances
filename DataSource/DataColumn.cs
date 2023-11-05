namespace DataSource;

public class DataColumn
{
    public string ColumnName { get; }

    public string DeepColumnName => ColumnName.Split('.').Last();

    public DataType ColumnDataType { get; }

    public DataColumn(string columnName, DataType columnDataType)
    {
        ColumnName = columnName;
        ColumnDataType = columnDataType;
    }
}
