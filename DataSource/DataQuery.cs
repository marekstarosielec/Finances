namespace DataSource;

public class DataQuery
{
    public Dictionary<DataColumn, bool>? Sort;
    public Dictionary<DataColumn, DataColumnFilter>? Filter;
    public int? PageSize;
}
