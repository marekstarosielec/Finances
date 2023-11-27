namespace DataView;

public class DataViewColumnDate : DataViewColumn
{
    public DataViewColumnDate(string dataColumnName, string title, string shortName, DateViewColumnDateFilterComponents? filterComponent = null) : base(dataColumnName, DataViewColumnDataType.Date, title, shortName)
    {
        if (filterComponent != null)
            PreferredFilterComponentType = Enum.GetName(filterComponent.Value);
    }
}
