namespace DataView;

public class DataViewColumnText : DataViewColumn
{
    public DataViewColumnText(string dataColumnName, string title, string shortName, DataViewColumnTextFilterComponents? filterComponent = null, int? numberOfLinesInDetails = null) : base(dataColumnName, DataViewColumnDataType.Text, title, shortName)
    {
        if (filterComponent != null)
            PreferredFilterComponentType = Enum.GetName(filterComponent.Value);
        NumberOfLinesInDetails = numberOfLinesInDetails;
    }
}
