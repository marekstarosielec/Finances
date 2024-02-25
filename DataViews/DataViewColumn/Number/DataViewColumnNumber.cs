namespace DataViews;

public class DataViewColumnNumber : DataViewColumn
{
    public DataViewColumnNumber(string dataColumnName, string title, string shortName) : base(dataColumnName, DataViewColumnDataType.Number, title, shortName)
    {
        HorizontalAlign = DataViewColumnContentAlign.Right;
    }
}
