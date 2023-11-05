namespace DataSource;

public class DataColumnMapping
{
    public string ColumnName { get; }

    public string? NewColumnName { get; }

    public DataColumnMapping(string columnName, string? newColumnName)
    {
        ColumnName = columnName;
        NewColumnName = newColumnName;
    }
}
