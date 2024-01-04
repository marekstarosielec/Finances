namespace DataSource;

public class DataRow : Dictionary<string, DataValue>
{
    public DataValue Id
    {
        get
        {
            return this["Id"];
        }
    }
}
