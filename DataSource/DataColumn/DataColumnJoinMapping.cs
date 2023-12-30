namespace DataSource;

public class DataColumnJoinMapping
{
    public string ColumnName { get; }

    public string? NewColumnName { get; }

    public DataColumnJoinMapping(string columnName, string? newColumnName)
    {
        ColumnName = columnName;
        NewColumnName = newColumnName;
    }
}
