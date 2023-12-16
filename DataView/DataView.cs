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

    public string? DetailsViewName { get; }


    private List<string> _checkedRecords = new List<string>();

    public string? SelectedRecordId { get; private set; }

    public void SelectRecord(Dictionary<DataColumn, object?>? row)
    {
        SelectedRecordId = GetRowId(row);
        _checkedRecords = new List<string>();
    }

    //public async Task<Dictionary<DataColumn, object?>?> GetSingleRecord(string id)
    //{
    //    var query = new DataQuery();
    //    query.Filters.Add(_dataSource.Columns["id"], new DataColumnFilter { StringValue = id });
    //    var result = await _dataSource.ExecuteQuery(query);
    //    return result.Rows.FirstOrDefault();
    //}

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
        SelectedRecordId = null;
        _checkedRecords.Add(GetRowId(row));
    }

    public void UncheckRecord(Dictionary<DataColumn, object?>? row)
    {
        _checkedRecords.RemoveAll(s => s == GetRowId(row));
    }

    public bool RecordIsChecked(Dictionary<DataColumn, object?>? row) => _checkedRecords.Any(s => s == GetRowId(row));

    public ReadOnlyCollection<string> CheckedRecords => _checkedRecords.AsReadOnly();

    public string Serialize()
    {
        var data = Query.Serialize();
        data["cr"] = string.Join(',', _checkedRecords);
        if (SelectedRecordId != null)
            data["sr"] = SelectedRecordId;
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

                _checkedRecords = cr.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            if (key == "sr")
            {
                var sr = items[key];
                SelectedRecordId = sr;
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
        DetailsViewName = detailsViewName;
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
}
