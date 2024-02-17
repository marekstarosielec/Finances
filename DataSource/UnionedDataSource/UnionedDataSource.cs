namespace DataSource;

public class UnionedDataSource : IDataSource
{
    private readonly IDataSource _firstDataSource;
    private readonly IDataSource _secondDataSource;
    private readonly Dictionary<DataColumn, DataColumnFilter> _mainFilters;
    private readonly DataColumnUnionMapping[] _mappings;
   
    public Dictionary<string, DataColumn> Columns { get; private set; }

    public string Id => $"{_firstDataSource.Id}_union_{_secondDataSource.Id}";

    private readonly DataSourceCacheStamp _cacheStamp;

    public UnionedDataSource(IDataSource firstDataSource, IDataSource secondDataSource, Dictionary<DataColumn, DataColumnFilter>? mainFilters = null, params DataColumnUnionMapping[] mappings)
    {
        _firstDataSource = firstDataSource;
        _secondDataSource = secondDataSource;
        _mainFilters = mainFilters ?? new();
        _mappings = mappings;
        Columns = GetColumnList();
        _cacheStamp = DataSourceCache.Instance.Register(Id, UnionTables, _firstDataSource.Id, _secondDataSource.Id);
    }

    //TODO: This method is same for JsonDataSource, JoinedDataSource and here. Need to extract it.
    public async Task<DataQueryResult> ExecuteQuery(DataQuery dataQuery)
    {
        var rows = await DataSourceCache.Instance.Get(Id, _cacheStamp);

        var allData = await DataSourceCache.Instance.Get(Id, _cacheStamp);
        var clonedData = allData.Clone();
        var clonedRows = clonedData.Rows;

        if (dataQuery.Filters != null)
            foreach (var filterDefinition in dataQuery.Filters)
                clonedRows = clonedRows.Filter(filterDefinition.Key, filterDefinition.Value).ToList();

        clonedRows = clonedRows.Sort(dataQuery.Sorters);
        var count = clonedRows.Count();

        if (dataQuery.PageSize.GetValueOrDefault(-1) > -1)
            clonedRows = clonedRows.Take(dataQuery.PageSize!.Value);

        //Remove columns (and its data) that are not specified in dataQuery. If no columns are specified (before joining or unioning tables), return all columns.
        var validColumns = clonedData.Columns.Where(c => dataQuery.Columns.Count == 0 || dataQuery.Columns.Any(dq => dq.ColumnName == c.ColumnName));
        var validRows = new List<DataRow>();
        foreach (var clonedRow in clonedRows)
        {
            var validRow = new DataRow();
            foreach (var dataColumn in validColumns)
                if (clonedRow.ContainsKey(dataColumn.ColumnName)) //Columns that are generated (e.g. Group) are not available here
                    validRow[dataColumn.ColumnName] = clonedRow[dataColumn.ColumnName];
            validRows.Add(validRow);
        }

        return new DataQueryResult(validColumns, validRows, count);
    }

    private async Task<DataQueryResult> UnionTables()
    {
        var result = new List<DataRow>();
        DataQueryResult firstDataView = await _firstDataSource.ExecuteQuery(new DataQuery {  PageSize = -1});
        foreach (var firstDataRow in firstDataView.Rows)
        {
            var resultDataRow = new DataRow();
            foreach (var mapping in _mappings)
                resultDataRow[mapping.ResultDataSourceColumnName] = mapping.FirstDataSourceColumnName == null ? new DataValue(null, null) : firstDataRow[mapping.FirstDataSourceColumnName];
            result.Add(resultDataRow);
        }

        DataQueryResult secondDataView = await _secondDataSource.ExecuteQuery(new DataQuery {  Filters = _mainFilters, PageSize = -1});
        foreach (var secondDataRow in secondDataView.Rows)
        {
            var resultDataRow = new DataRow();
            foreach (var mapping in _mappings)
                resultDataRow[mapping.ResultDataSourceColumnName] = mapping.SecondDataSourceColumnName == null ? new DataValue(null, null) : secondDataRow[mapping.SecondDataSourceColumnName];
            result.Add(resultDataRow);
        }

        return new DataQueryResult(Columns.Values, result, result.Count);
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
        DataSourceCache.Instance.Clean(Id);
    }

    public Task Save(List<DataRow> rows)
    {
        throw new NotImplementedException();
    }
}
