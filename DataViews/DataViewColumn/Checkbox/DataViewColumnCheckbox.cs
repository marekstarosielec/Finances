namespace DataViews;

public class DataViewColumnCheckbox : DataViewColumn
{
    public DataViewColumnCheckbox(string dataColumnName, string title, string shortName, bool visible = true) : base(dataColumnName, DataViewColumnDataType.Checkbox, title, shortName, visible: visible)
    {
    }
}
