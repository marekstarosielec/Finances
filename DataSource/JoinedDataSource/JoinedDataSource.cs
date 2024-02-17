using System.Data;
using System.Data.Common;

namespace DataSource;

public class JoinedDataSource : IDataSource
{
    private readonly IDataSource _leftDataSource;
    private readonly IDataSource _rightDataSource;
    private readonly string _joinColumn;
    private readonly DataColumnJoinMapping[] _mappings;
    
    public Dictionary<string, DataColumn> Columns { get; private set; }

    public string Id => $"{_leftDataSource.Id}_join_{_rightDataSource.Id}";

    private readonly DataSourceCacheStamp _cacheStamp;
    private readonly DataQueryExecutor _dataQueryExecutor = new();

    public JoinedDataSource(IDataSource leftDataSource, IDataSource rightDataSource, string joinColumn, params DataColumnJoinMapping[] mappings)
    {
        _leftDataSource = leftDataSource;
        _rightDataSource = rightDataSource;
        _joinColumn = joinColumn;
        _mappings = mappings;
        Columns = BuildJoinedColumnList();
        _cacheStamp = DataSourceCache.Instance.Register(Id, LeftJoinTable, _leftDataSource.Id, _rightDataSource.Id);
    }

    public Task<DataQueryResult> ExecuteQuery(DataQuery dataQuery)
        => _dataQueryExecutor.ExecuteQuery(Id, _cacheStamp, dataQuery);

    private async Task<DataQueryResult> LeftJoinTable()
    {
        DataQueryResult leftDataView = await _leftDataSource.ExecuteQuery(new DataQuery { PageSize = -1 });
        DataQueryResult rightDataView = await _rightDataSource.ExecuteQuery(new DataQuery { PageSize = -1 });
        DataColumn? leftDataViewJoinColumn;
        DataColumn? rightDataViewJoinColumn;
        if (_leftDataSource.Columns.ContainsKey(_joinColumn))
        {
            leftDataViewJoinColumn = _leftDataSource.Columns[_joinColumn];
            rightDataViewJoinColumn = _rightDataSource.Columns["Id"];
        }
        else if (_rightDataSource.Columns.ContainsKey(_joinColumn))
        {
            leftDataViewJoinColumn = _leftDataSource.Columns["Id"];
            rightDataViewJoinColumn = _rightDataSource.Columns[_joinColumn];
        }
        else
            throw new InvalidOperationException($"Joined data source does not contain column {_joinColumn}");

        foreach (var leftDataRow in leftDataView.Rows)
        {
            if (leftDataRow[leftDataViewJoinColumn.ColumnName] == null)
                //If join column is not filled, columns from right table are all null.
                JoinRightRow(leftDataRow, null);
            else
            {
                //Find matching row in right table.
                var matching = rightDataView.Rows.FirstOrDefault(r => r[rightDataViewJoinColumn.ColumnName].OriginalValue as string == leftDataRow[leftDataViewJoinColumn.ColumnName].OriginalValue as string);
                if (matching == null)
                    //No matching row in right table found, fill columns with null.
                    JoinRightRow(leftDataRow, null); 
                else
                    //Fill columns with data from matching row.
                    JoinRightRow(leftDataRow, matching);
            }
        }

        var result = new List<DataRow>();
        foreach (var row in leftDataView.Rows)
        {
            var newRow = new DataRow();
            foreach (var column in Columns)
                if (row.ContainsKey(column.Value.ColumnName)) //Columns that are generated (e.g. Group) are not available here
                    newRow[column.Value.ColumnName] = row[column.Value.ColumnName];
            result.Add(newRow);
        }
        return new DataQueryResult(Columns.Values, result, result.Count);
    }

    private void JoinRightRow(DataRow leftDataRow, DataRow? rightDataRow)
    {
        foreach (var column in _rightDataSource.Columns)
        {
            var originalValue = rightDataRow?[column.Value.ColumnName].OriginalValue;
            var originalDataValue = new DataValue(originalValue, originalValue);
            var mapping = _mappings?.FirstOrDefault(c => c.ColumnName == column.Key);
            if (mapping == null)
            {
                //No information about mapping, so just adding column to result.
                if (!leftDataRow.ContainsKey(column.Value.ColumnName))
                    leftDataRow.Add(column.Value.ColumnName, originalDataValue);
                else
                    leftDataRow[column.Value.ColumnName] = originalDataValue;
            }
            else if (mapping.NewColumnName == null)
                continue; //Passed null as NewColumnName means column should not be included in joined result set.
            else
            {
                //Mapping contains NewColumnName, use it instead of previous name.
                if (!leftDataRow.ContainsKey(mapping.NewColumnName))
                    leftDataRow.Add(mapping.NewColumnName, originalDataValue);
                else
                    leftDataRow[mapping.NewColumnName] = originalDataValue;
            }
        }
    }

    private Dictionary<string, DataColumn> BuildJoinedColumnList()
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
        DataSourceCache.Instance.Clean(Id);
    }

    public Task Save(List<DataRow> rows)
    {
        throw new NotImplementedException();
    }
}
