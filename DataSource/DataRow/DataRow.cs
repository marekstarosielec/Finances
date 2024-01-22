namespace DataSource;

public class DataRow : Dictionary<string, DataValue>
{
    public DataValue Id => this["Id"];

    public DataValue? GroupId => ContainsKey("GroupId") ? this["GroupId"] : null;

    public bool SelectedInDetails {get; set;}
}
