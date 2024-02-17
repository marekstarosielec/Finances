namespace DataSource;

public static class GroupDataSource
{
    public static string Id { get => _group.Id ?? throw new InvalidOperationException("DataSource is not created yet"); }

    private static IDataSource? _group = null;

    internal static void Create(string dataFilePath)
    {
        if (_group != null) 
            return;
        _group = new JsonDataSource(dataFilePath, "group.json",
            new IdDataColumn(),
            new DataColumn("GroupId", ColumnDataType.Text),
            new DataColumn("DataViewName", ColumnDataType.Text),
            new DataColumn("RowId", ColumnDataType.Text)
            );
    }

    public static IDataSource Instance { get => _group ?? throw new InvalidOperationException("DataSource is not created yet"); }
}
