namespace DataSource;

public class DataRow : Dictionary<string, DataValue>
{
    public DataValue Id => this["Id"];

    //public DataValue? Group => ContainsKey(GroupDataColumn.Name) ? this[GroupDataColumn.Name] : null;

    public string? GroupId => ContainsKey(GroupDataColumn.Name) ? (this[GroupDataColumn.Name].CurrentValue as GroupDataValue)?.GroupId : null;

    public bool SelectedInDetails {get; set;}
}
