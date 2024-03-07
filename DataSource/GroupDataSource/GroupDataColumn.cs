namespace DataSource;

/// <summary>
/// Groupping column containing subquery available in other DataSources.
/// </summary>
public class GroupDataColumn : DataColumn
{
    public const string Name = "Group";
    private const ColumnDataType DataType = ColumnDataType.Subquery;

    public static bool IsGroupColumn(DataColumn column) => column.ColumnName == Name && column.ColumnDataType == DataType;

    public GroupDataColumn() : base(Name, DataType)
    {
    }
}
