namespace DataSource;

public class UnionedDataSource : IDataSource
{
    private readonly IDataSource _firstDataSource;
    private readonly IDataSource _secondDataSource;
    private readonly Dictionary<DataColumn, DataColumnFilter> _mainFilters;
    private readonly DataColumnUnionMapping[] _mappings;
    public UnionedDataSourceCache Cache { get; } = new();
    public DateTime? CacheTimeStamp => Cache.TimeStamp;

    public Dictionary<string, DataColumn> Columns { get; private set; }

    public UnionedDataSource(IDataSource firstDataSource, IDataSource secondDataSource, Dictionary<DataColumn, DataColumnFilter>? mainFilters = null, params DataColumnUnionMapping[] mappings)
    {
        _firstDataSource = firstDataSource;
        _secondDataSource = secondDataSource;
        _mainFilters = mainFilters ?? new();
        _mappings = mappings;
        Columns = GetColumnList();
    }

    public async Task<DataQueryResult> ExecuteQuery(DataQuery? dataQuery = null)
    {
        IEnumerable<DataRow> rows = await Cache.Get(_firstDataSource, _secondDataSource, UnionTables);

        if (dataQuery != null)
            rows = rows.Sort(dataQuery.Sorters);

        if (dataQuery?.Filters != null)
            foreach (var filterDefinition in dataQuery.Filters)
                rows = rows.Filter(filterDefinition.Key, filterDefinition.Value);

        var totalRowCount = rows.Count();

        if (dataQuery?.PageSize.GetValueOrDefault(-1) > -1)
            rows = rows.Take(dataQuery.PageSize!.Value);

        return new DataQueryResult(Columns.Values, rows, totalRowCount);
    }

    private async Task<IEnumerable<DataRow>> UnionTables()
    {
        var result = new List<DataRow>();
        DataQueryResult firstDataView = await _firstDataSource.ExecuteQuery(new DataQuery {  PageSize = -1});
        foreach (var firstDataRow in firstDataView.Rows)
        {
            var resultDataRow = new DataRow();
            foreach (var mapping in _mappings)
                resultDataRow[mapping.ResultDataSourceColumnName] = mapping.FirstDataSourceColumnName == null ? new DataValue(null) : firstDataRow[mapping.FirstDataSourceColumnName];
            result.Add(resultDataRow);
        }

        DataQueryResult secondDataView = await _secondDataSource.ExecuteQuery(new DataQuery {  Filters = _mainFilters, PageSize = -1});
        foreach (var secondDataRow in secondDataView.Rows)
        {
            var resultDataRow = new DataRow();
            foreach (var mapping in _mappings)
                resultDataRow[mapping.ResultDataSourceColumnName] = mapping.SecondDataSourceColumnName == null ? new DataValue(null) : secondDataRow[mapping.SecondDataSourceColumnName];
            result.Add(resultDataRow);
        }
        
        return result;
    }

    private Dictionary<string, DataColumn> GetColumnList()
    {
        var result = new Dictionary<string, DataColumn>();
        foreach (var mapping in _mappings)
        {
            var columnDataType = mapping.FirstDataSourceColumnName != null ? _firstDataSource.Columns[mapping.FirstDataSourceColumnName].ColumnDataType : _secondDataSource.Columns[mapping.SecondDataSourceColumnName!].ColumnDataType;
            var dataColumn = new DataColumn(mapping.ResultDataSourceColumnName, columnDataType);
            result.Add(mapping.ResultDataSourceColumnName, dataColumn);
        }
        return result;
    }

    public void RemoveCache()
    {
        Cache.Clean();
    }

    public Task Save(DataRow row)
    {
        throw new NotImplementedException();
    }
}
