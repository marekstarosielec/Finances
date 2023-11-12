namespace DataSource;

public class DataQuery
{
    public Dictionary<DataColumn, bool> Sorters = new();
    public Dictionary<DataColumn, DataColumnFilter> Filters = new();
    public int? PageSize = 100;
}
