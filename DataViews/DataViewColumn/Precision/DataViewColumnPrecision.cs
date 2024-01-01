namespace DataViews;

public class DataViewColumnPrecision : DataViewColumn
{
    public DataViewColumnPrecision(string dataColumnName, string title, string shortName) : base(dataColumnName, DataViewColumnDataType.Precision, title, shortName)
    {
        HorizontalAlign = DataViewColumnContentAlign.Right;
        Format = "0.00";
    }
}
