namespace DataSource;

public class GroupDataColumn : DataColumn
{
    private const string Name = "Group";
    private const ColumnDataType DataType = ColumnDataType.Subquery;

    public static bool IsGroupColumn(DataColumn column) => column.ColumnName == Name && column.ColumnDataType == DataType;

    public GroupDataColumn() : base(Name, DataType)
    {
    }
}
