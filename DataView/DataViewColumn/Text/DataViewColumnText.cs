namespace DataView;

public class DataViewColumnText : DataViewColumn
{
    public DataViewColumnText(string dataColumnName, string title, string shortName) : base(dataColumnName, DataViewColumnDataType.Text, title, shortName)
    {
    }
}
