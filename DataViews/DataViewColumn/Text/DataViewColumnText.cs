namespace DataViews;

public class DataViewColumnText : DataViewColumn
{
    public DataViewColumnText(string dataColumnName, string title, string shortName, DataViewColumnTextFilterComponents? filterComponent = null, int? numberOfLinesInDetails = null, bool visible = true) : base(dataColumnName, DataViewColumnDataType.Text, title, shortName, visible: visible)
    {
        if (filterComponent != null)
            PreferredFilterComponentType = Enum.GetName(filterComponent.Value);
        NumberOfLinesInDetails = numberOfLinesInDetails;
    }
}
