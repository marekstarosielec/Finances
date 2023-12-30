namespace DataSource;

public class DataColumnUnionMapping
{
    public string ResultDataSourceColumnName { get; }

    public string? FirstDataSourceColumnName { get; }

    public string? SecondDataSourceColumnName { get; }

    public DataColumnUnionMapping(string resultDataSourceColumnName, string? firstDataSourceColumnName, string? secondDataSourceColumnName)
    {
        ResultDataSourceColumnName = resultDataSourceColumnName;
        FirstDataSourceColumnName = firstDataSourceColumnName;
        SecondDataSourceColumnName = secondDataSourceColumnName;
    }
}
