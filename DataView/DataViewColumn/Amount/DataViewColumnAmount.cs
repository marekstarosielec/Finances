namespace DataView;

public class DataViewColumnAmount : DataViewColumn
{
    public DataViewColumnAmount(string dataColumnName, string currencyColumn, string title, string shortName) : base(dataColumnName, DataViewColumnDataType.Precision, title, shortName)
    {
        SecondaryDataColumnName = currencyColumn;
    }
}
