namespace DataSource;

public class GroupDataValue
{
    public string GroupId { get; set; }

    public IEnumerable<DataRow> GroupRows { get; set; }

    public GroupDataValue(string groupId, IEnumerable<DataRow> groupRows)
    {
        GroupId = groupId;
        GroupRows = groupRows;
    }
}
