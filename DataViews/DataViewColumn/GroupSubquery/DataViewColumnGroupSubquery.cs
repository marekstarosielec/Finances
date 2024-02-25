namespace DataViews;

public class DataViewColumnGroupSubquery : DataViewColumn
{
    public DataViewColumnGroupSubquery(string dataColumnName, string title, string shortName) : base(dataColumnName, DataViewColumnDataType.GroupSubquery, title, shortName)
    {
    }
}
