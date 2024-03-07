namespace DataSource;

/// <summary>
/// Column containind di of group (if any), that record belongs to, available in other DataSources.
/// </summary>
public class GroupIdDataColumn : DataColumn
{
    public const string Name = "GroupId";
    private const ColumnDataType DataType = ColumnDataType.Text;

    public GroupIdDataColumn() : base(Name, DataType)
    {
    }
}
