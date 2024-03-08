namespace DataSource;

public static class GroupDataSource
{
    public static string Id { get => _group?.Id ?? throw new InvalidOperationException("DataSource is not created yet"); }

    private static IDataSource? _group = null;

    public static DataColumn RowIdDataColumn => new("RowId", ColumnDataType.Text);
    public static DataColumn GroupIdDataColumn => new("GroupId", ColumnDataType.Text);
    public static string FileLinkColumnId => "FileLink";

    internal static void Create(string dataFilePath)
    {
        if (_group != null)
            return;
        _group = new JsonDataSource(dataFilePath, "group.json",
            includeGroups: false,
            new IdDataColumn(),
            GroupIdDataColumn,
            new DataColumn("DataViewName", ColumnDataType.Text),
            RowIdDataColumn,
            new DataColumn("DocumentNumber", ColumnDataType.Number),
            new DataColumn(FileLinkColumnId, ColumnDataType.Text, r => CreateDocumentFullName(r, dataFilePath))
        );
    }

    public static IDataSource Instance { get => _group ?? throw new InvalidOperationException("DataSource is not created yet"); }

    private static string? CreateDocumentFullName(DataRow row, string dataFilePath)
    {
        var number = row["DocumentNumber"].OriginalValue?.ToString();
        if (string.IsNullOrWhiteSpace(number))
            return null;

        var fileNumber = $"MX{number.PadLeft(5, '0')}.zip";
        return Path.Combine(dataFilePath, "documents", fileNumber);
    }
}