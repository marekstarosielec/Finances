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


    private List<string> _selectedRecords = new List<string>();

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

    public void SelectRow(Dictionary<DataColumn, object?>? row)
    {
        _selectedRecords.Add(GetRowId(row));
    }

    public void UnselectRow(Dictionary<DataColumn, object?>? row)
    {
        _selectedRecords.RemoveAll(s => s == GetRowId(row));
    }

    public bool RowIsSelected(Dictionary<DataColumn, object?>? row) => _selectedRecords.Any(s => s == GetRowId(row));

    public string Serialize()
    {
        var data = Query.Serialize();
       // data["sr"] = string.Join(',', _selectedRecords);
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

            if (key == "sr")
            {
                var sr = items[key];
                if (sr == null)
                    return;

                //_selectedRecords = sr.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
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
