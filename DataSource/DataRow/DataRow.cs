namespace DataSource;

public class DataRow : Dictionary<string, DataValue>
{
    public DataValue Id => this["Id"];

    public DataValue? Group => ContainsKey(GroupDataColumn.Name) ? this[GroupDataColumn.Name] : null;

    public DataValue? GroupId => ContainsKey(GroupIdDataColumn.Name) ? this[GroupIdDataColumn.Name] : null;

    public bool SelectedInDetails {get; set;}
}
