using DataSource;
using System.Collections.ObjectModel;
using System.Web;

namespace DataView;

public class DataView
{
    private IDataSource _dataSource;

    private DataQuery _query = new DataQuery();

    public string Name { get; }

    public string Title { get; set; }

    public ReadOnlyCollection<DataViewColumn> Columns { get; }

    public DataViewPresentation? Presentation { get; }

    public DataViewQuery Query { get; init; }

    public bool IsLoading { get; private set; } = true;

    public DataQueryResult? Result { get; private set; }

    private string? _detailsViewName { get; }


    private Dictionary<string,string> _checkedRecords = new Dictionary<string, string>();

    private string GetRowId(Dictionary<DataColumn, object?>? row)
    {
        if (row == null)
            throw new ArgumentNullException(nameof(row));

        var idColumn = _dataSource.Columns.FirstOrDefault(c => c.Key == "Id").Value;
        if (idColumn == null)
            throw new InvalidOperationException("Cannot find Id column in data source");

        var id = row[idColumn]?.ToString();
        if (id == null)
            throw new InvalidOperationException("Cannot find row id");
        
        return id;
    }

    public void CheckRecord(Dictionary<DataColumn, object?>? row)
    {
        var detailsView = GetDetailsDataViewName();
        if (detailsView == null)
            return;
        _checkedRecords.Add(GetRowId(row), detailsView);
    }

    public void UncheckRecord(Dictionary<DataColumn, object?>? row)
    {
        _checkedRecords.Remove(GetRowId(row));
    }

    public void UncheckRecords()
    {
        _checkedRecords.Clear();
    }

    public bool RecordIsChecked(Dictionary<DataColumn, object?>? row) => _checkedRecords.ContainsKey(GetRowId(row));

    public ReadOnlyDictionary<string, string> CheckedRecords => new ReadOnlyDictionary<string, string>(_checkedRecords);

    public string Serialize()
    {
        var data = Query.Serialize();
        data["cr"] = string.Join(',', _checkedRecords.Keys);
        return new StreamReader(new FormUrlEncodedContent(data).ReadAsStream()).ReadToEnd();
    }

    public void Deserialize(string serializedValue)
    {
        if (serializedValue == null)
            return;

        var items = HttpUtility.ParseQueryString(serializedValue);
        Query.Deserialize(items);

        foreach (string key in items)
        {
            if (key == null)
                continue;

            if (key == "cr")
            {
                var cr = items[key];
                if (cr == null)
                    return;

                var checkedRecordsIds = cr.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var checkedRecordId in checkedRecordsIds)
                {
                    var detailsView = GetDetailsDataViewName();
                    if (detailsView == null)
                        continue;
                    _checkedRecords[checkedRecordId] = detailsView;
                }
            }
        }
    }

    public void RemoveCache()
    {
        _dataSource.RemoveCache();
        IsLoading = true;
    }

    public async Task Requery()
    {
        IsLoading = true;
        Query.Apply();
        Result = await _dataSource.ExecuteQuery(_query);
        IsLoading = false;
    }

    public DataView(string name, string title, IDataSource dataSource, ReadOnlyCollection<DataViewColumn> columns, DataViewPresentation? presentation = null, string? detailsViewName = null)
    {
        Name = name;
        Title = title;
        _dataSource = dataSource;
        Presentation = presentation;
        Columns = columns;
        Query = new DataViewQuery(_query, _dataSource, Columns);
        ValidateColumns();
        _detailsViewName = detailsViewName;
    }

    void ValidateColumns()
    {
        foreach (DataViewColumn column in Columns)
        {
            if (!_dataSource.Columns.ContainsKey(column.PrimaryDataColumnName))
                throw new InvalidOperationException($"DataViewColumn {column.ShortName} refers to DataColumn {column.PrimaryDataColumnName} which does not exist.");
            if (column.SecondaryDataColumnName != null && !_dataSource.Columns.ContainsKey(column.SecondaryDataColumnName))
                throw new InvalidOperationException($"DataViewColumn {column.ShortName} refers to DataColumn {column.SecondaryDataColumnName} which does not exist.");
        }
    }

    public string? GetDetailsDataViewName()
    {
        return _detailsViewName;
    }
}
