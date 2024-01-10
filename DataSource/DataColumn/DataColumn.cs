namespace DataSource;

public class DataColumn
{
    public string ColumnName { get; }

    public ColumnDataType ColumnDataType { get; }
    public Func<DataRow, object?>? CustomCreator { get; }

    public DataColumn(string columnName, ColumnDataType columnDataType, Func<DataRow, object?>? customCreator = null)
    {
        ColumnName = columnName;
        ColumnDataType = columnDataType;
        CustomCreator = customCreator;
    }
}
