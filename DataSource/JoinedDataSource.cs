﻿namespace DataSource;

public class JoinedDataSource : IDataSource
{
    private readonly IDataSource _leftDataSource;
    private readonly IDataSource _rightDataSource;
    private readonly string _joinColumn;
    private readonly DataColumnJoinMapping[] _mappings;

    public Dictionary<string, DataColumn> Columns { get; private set; }

    public JoinedDataSource(IDataSource leftDataSource, IDataSource rightDataSource, string joinColumn, params DataColumnJoinMapping[] mappings)
    {
        _leftDataSource = leftDataSource;
        _rightDataSource = rightDataSource;
        _joinColumn = joinColumn;
        _mappings = mappings;
        Columns = GetColumnList();
    }
    
    public async Task<DataQueryResult> ExecuteQuery(DataQuery? dataQuery = null)
    {
        IEnumerable<Dictionary<DataColumn, object?>> rows = await LeftJoinTable();
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

    private async Task<IEnumerable<Dictionary<DataColumn, object?>>> LeftJoinTable()
    {
        DataQueryResult leftDataView = await _leftDataSource.ExecuteQuery(new DataQuery());
        DataQueryResult rightDataView = await _rightDataSource.ExecuteQuery(new DataQuery());
        var leftDataViewJoinColumn = _leftDataSource.Columns.ContainsKey(_joinColumn) ? _leftDataSource.Columns[_joinColumn] : throw new InvalidOperationException($"Joined data source does not contain column {_joinColumn}");
        var rightDataViewJoinColumn = _rightDataSource.Columns["Id"];

        foreach (var leftDataRow in leftDataView.Rows)
        {
            if (leftDataRow[leftDataViewJoinColumn] == null)
                JoinRightRow(leftDataRow, null);

            var matching = rightDataView.Rows.FirstOrDefault(r => r[rightDataViewJoinColumn] as string == leftDataRow[leftDataViewJoinColumn] as string);
            if (matching == null)
                JoinRightRow(leftDataRow, null);

            JoinRightRow(leftDataRow, matching);
        }

        var result = new List<Dictionary<DataColumn, object?>>();
        foreach (var row in leftDataView.Rows)
        {
            var newRow = new Dictionary<DataColumn, object?>();
            foreach (var column in Columns)
                newRow[column.Value] = row[column.Value];
            result.Add(newRow);
        }
        return result;
    }

    private void JoinRightRow(Dictionary<DataColumn, object?> leftDataRow, Dictionary<DataColumn, object?>? rightDataRow)
    {
        foreach (var column in _rightDataSource.Columns)
        {
            var mapping = _mappings?.FirstOrDefault(c => c.ColumnName == column.Key);
            if (mapping == null)
                leftDataRow[column.Value] = column.Value;
            else if (mapping.NewColumnName == null)
                continue;
            else
                leftDataRow[Columns[mapping.NewColumnName]] = rightDataRow?[column.Value];
        }
    }

    private Dictionary<string, DataColumn> GetColumnList()
    {
        var result = new Dictionary<string, DataColumn>();
        foreach (var column in _leftDataSource.Columns)
            result[column.Key] = column.Value;
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
}