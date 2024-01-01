using DataSource;
using DataViews;
using System.Collections.ObjectModel;

namespace FinancesBlazor;

public class SelectedData
{
    private Dictionary<string, DataView> _ids = new();

    private ReadOnlyDictionary<string, DataView>? _readOnlyIds;
    public ReadOnlyDictionary<string, DataView> Ids
    {
        get
        {
            _readOnlyIds ??= new(_ids);
            return _readOnlyIds;
        }
    }

    public void Clear() => _ids.Clear();

    public void Add(DataView dataView, DataRow row)
    {
        _ids.Add(GetRowId(dataView, row), dataView);
    }

    public void Add(DataView dataView, string id)
    {
        _ids.Add(id, dataView);
    }

    public void Remove(DataView dataView, DataRow row)
    {
        _ids.Remove(GetRowId(dataView, row));
    }

    public bool IsSelected(DataView dataView, DataRow row) => _ids.ContainsKey(GetRowId(dataView, row));

    private string GetRowId(DataView dataView, DataRow row)
    {
        if (row == null)
            throw new ArgumentNullException(nameof(row));

        var idColumn = dataView.DataSource.Columns.FirstOrDefault(c => c.Key == "Id").Value;
        if (idColumn == null)
            throw new InvalidOperationException("Cannot find Id column in data source");

        var id = row[idColumn]?.OriginalValue?.ToString();
        if (id == null)
            throw new InvalidOperationException("Cannot find row id");

        return id;
    }
}
