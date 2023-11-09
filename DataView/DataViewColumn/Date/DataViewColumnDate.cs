namespace DataView;

public class DataViewColumnDate : DataViewColumn
{
    public DataViewColumnDate(string dataColumnName, string title, string shortName) : base(dataColumnName, DataViewColumnDataType.Date, title, shortName)
    {
    }
}
