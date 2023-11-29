namespace DataView;

public class DataViewColumnAmount : DataViewColumn
{
    public DataViewColumnAmount(string dataColumnName, string currencyColumn, string title, string shortName) : base(dataColumnName, DataViewColumnDataType.Amount, title, shortName)
    {
        SecondaryDataColumnName = currencyColumn;
        HorizontalAlign = DataViewColumnContentAlign.Right;
        Format = "0.00";
    }
}
