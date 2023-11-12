namespace DataSource;

public class UnionedDataSource : IDataSource
{
    private readonly IDataSource _firstDataSource;
    private readonly IDataSource _secondDataSource;
    private readonly DataColumnUnionMapping[] _mappings;

    public Dictionary<string, DataColumn> Columns { get; private set; }

    public UnionedDataSource(IDataSource firstDataSource, IDataSource secondDataSource, params DataColumnUnionMapping[] mappings)
    {
        _firstDataSource = firstDataSource;
        _secondDataSource = secondDataSource;
        _mappings = mappings;
        Columns = GetColumnList();
    }

    public async Task<DataQueryResult> ExecuteQuery(DataQuery? dataQuery = null)
    {
        IEnumerable<Dictionary<DataColumn, object?>> rows = await UnionTables();

        if (dataQuery?.Sorters != null)
            foreach (var sortDefinition in dataQuery.Sorters)
                rows = rows.Sort(sortDefinition.Key, sortDefinition.Value);
        
        if (dataQuery?.Filters != null)
            foreach (var filterDefinition in dataQuery.Filters)
                rows = rows.Fitler(filterDefinition.Key, filterDefinition.Value);

        var totalRowCount = rows.Count();

        if (dataQuery?.PageSize.GetValueOrDefault(-1) > -1)
            rows = rows.Take(dataQuery.PageSize!.Value);

        return new DataQueryResult(Columns.Values, rows, totalRowCount);
    }

    private async Task<IEnumerable<Dictionary<DataColumn, object?>>> UnionTables()
    {
        var result = new List<Dictionary<DataColumn, object?>>();
        DataQueryResult firstDataView = await _firstDataSource.ExecuteQuery(new DataQuery());
        foreach (var firstDataRow in firstDataView.Rows)
        {
            var resultDataRow = new Dictionary<DataColumn, object?>();
            foreach (var mapping in _mappings)
                resultDataRow[Columns[mapping.ResultDataSourceColumnName]] = mapping.FirstDataSourceColumnName == null ? null : firstDataRow[_firstDataSource.Columns[mapping.FirstDataSourceColumnName]];
            result.Add(resultDataRow);
        }

        DataQueryResult secondDataView = await _secondDataSource.ExecuteQuery(new DataQuery());
        foreach (var secondDataRow in secondDataView.Rows)
        {
            var resultDataRow = new Dictionary<DataColumn, object?>();
            foreach (var mapping in _mappings)
                resultDataRow[Columns[mapping.ResultDataSourceColumnName]] = mapping.SecondDataSourceColumnName == null ? null : secondDataRow[_secondDataSource.Columns[mapping.SecondDataSourceColumnName]];
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
}
