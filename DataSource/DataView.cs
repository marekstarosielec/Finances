namespace DataSource;

public class DataView
{
    public Dictionary<DataColumn, bool>? Sort;
    public Dictionary<DataColumn, DataColumnFilter>? Filter;
    public int? PageSize;
}
