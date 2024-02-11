namespace DataSource;

public class GroupDataSource
{
    private static IDataSource? _group = null;
    public static IDataSource GetInstance(string dataFilePath)
    {
        if (string.IsNullOrWhiteSpace(dataFilePath))
            throw new ArgumentNullException(nameof(dataFilePath));

        return _group ??= new JsonDataSource(Path.Combine(dataFilePath, "group.json"),
            new IdDataColumn(),
            new DataColumn("GroupId", ColumnDataType.Text),
            new DataColumn("DataViewName", ColumnDataType.Text),
            new DataColumn("RowId", ColumnDataType.Text)
            );
    }
}
