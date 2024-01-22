namespace DataSource;

public class JoinedDataSource : IDataSource
{
    private readonly IDataSource _leftDataSource;
    private readonly IDataSource _rightDataSource;
    private readonly string _joinColumn;
    private readonly DataColumnJoinMapping[] _mappings;
    public JoinedDataSourceCache Cache { get; } = new ();
    public DateTime? CacheTimeStamp => Cache.TimeStamp;

    public Dictionary<string, DataColumn> Columns { get; private set; }

    public JoinedDataSource(IDataSource leftDataSource, IDataSource rightDataSource, string joinColumn, params DataColumnJoinMapping[] mappings)
    {
        _leftDataSource = leftDataSource;
        _rightDataSource = rightDataSource;
        _joinColumn = joinColumn;
        _mappings = mappings;
        Columns = GetColumnList();
    }
    
    public async Task<DataQueryResult> ExecuteQuery(DataQuery dataQuery)
    {
        IEnumerable<DataRow> rows = await Cache.Get(_leftDataSource, _rightDataSource, LeftJoinTable);

        if (dataQuery?.Filters != null)
            foreach (var filterDefinition in dataQuery.Filters)
                rows = rows.Filter(filterDefinition.Key, filterDefinition.Value);

        if (dataQuery != null)
            rows = rows.Sort(dataQuery.Sorters);

        var totalRowCount = rows.Count();
        
        if (dataQuery?.PageSize.GetValueOrDefault(-1) > -1)
            rows = rows.Take(dataQuery.PageSize!.Value);

        return new DataQueryResult(Columns.Values, rows, totalRowCount);
    }

    private async Task<IEnumerable<DataRow>> LeftJoinTable()
    {
        DataQueryResult leftDataView = await _leftDataSource.ExecuteQuery(new DataQuery{  PageSize = -1 });
        DataQueryResult rightDataView = await _rightDataSource.ExecuteQuery(new DataQuery{  PageSize = -1 });
        var leftDataViewJoinColumn = _leftDataSource.Columns.ContainsKey(_joinColumn) ? _leftDataSource.Columns[_joinColumn] : throw new InvalidOperationException($"Joined data source does not contain column {_joinColumn}");
        var rightDataViewJoinColumn = _rightDataSource.Columns["Id"];

        foreach (var leftDataRow in leftDataView.Rows)
        {
            if (leftDataRow[leftDataViewJoinColumn.ColumnName] == null)
                JoinRightRow(leftDataRow, null);

            var matching = rightDataView.Rows.FirstOrDefault(r => r[rightDataViewJoinColumn.ColumnName].OriginalValue as string == leftDataRow[leftDataViewJoinColumn.ColumnName].OriginalValue as string);
            if (matching == null)
                JoinRightRow(leftDataRow, null);

            JoinRightRow(leftDataRow, matching);
        }

        var result = new List<DataRow>();
        foreach (var row in leftDataView.Rows)
        {
            var newRow = new DataRow();
            foreach (var column in Columns)
                newRow[column.Value.ColumnName] = row[column.Value.ColumnName];
            result.Add(newRow);
        }
        return result;
    }

    private void JoinRightRow(DataRow leftDataRow, DataRow? rightDataRow)
    {
        foreach (var column in _rightDataSource.Columns)
        {
            var mapping = _mappings?.FirstOrDefault(c => c.ColumnName == column.Key);
            if (mapping == null)
                leftDataRow[column.Value.ColumnName] ??= new DataValue(column.Value);
            else if (mapping.NewColumnName == null)
                continue;
            else
                leftDataRow[mapping.NewColumnName] = rightDataRow?[column.Value.ColumnName] ?? new DataValue(null);
        }
    }

    private Dictionary<string, DataColumn> GetColumnList()
    {
        var result = new Dictionary<string, DataColumn>();
        
        //Use all columns from left data source
        foreach (var column in _leftDataSource.Columns)
            result[column.Key] = column.Value;
        
        //Use all columns from right data source
        foreach (var column in _rightDataSource.Columns)
        {
            var mapping = _mappings?.FirstOrDefault(c => c.ColumnName == column.Key);
            if (mapping == null)
                result[column.Key] = column.Value;
            else if (mapping.NewColumnName == null)
                continue;
            else
                result[mapping.NewColumnName] = new DataColumn(mapping.NewColumnName, column.Value.ColumnDataType);
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
