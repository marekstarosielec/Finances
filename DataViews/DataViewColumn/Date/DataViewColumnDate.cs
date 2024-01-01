namespace DataViews;

public class DataViewColumnDate : DataViewColumn
{
    public DataViewColumnDate(string dataColumnName, string title, string shortName, DataViewColumnDateFilterComponents? filterComponent = null) : base(dataColumnName, DataViewColumnDataType.Date, title, shortName)
    {
        if (filterComponent != null)
            PreferredFilterComponentType = Enum.GetName(filterComponent.Value);
    }
}
