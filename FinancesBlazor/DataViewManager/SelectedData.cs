using DataSource;
using DataViews;
using System.Collections.ObjectModel;

namespace FinancesBlazor;

public class SelectedData
{
    private Dictionary<string, DataView> _ids = new();

    private ReadOnlyDictionary<string, DataView>? _readOnlyIds;
    public ReadOnlyDictionary<string, DataView> Ids => _readOnlyIds ??= new(_ids);

    public event EventHandler? Changed;
    public event EventHandler? DetailsCollapsedChanged;

    public bool DetailsCollapsed { get; set; }

    internal void InvokeDetailsCollapsedChanged() => DetailsCollapsedChanged?.Invoke(this, EventArgs.Empty);

    public void Clear() => _ids.Clear();

    public void Add(DataView dataView, DataRow row)
    {
        _ids.Add(GetRowId(row), dataView);
    }

    public void Add(DataView dataView, string id)
    {
        _ids.Add(id, dataView);
    }

    public void Remove(DataRow row)
    {
        _ids.Remove(GetRowId(row));
    }

    public bool IsSelected(DataRow row) => _ids.ContainsKey(GetRowId(row));

    internal void InvokeChanged() => Changed?.Invoke(this, EventArgs.Empty);
    
    private string GetRowId(DataRow row)
    {
        if (row == null)
            throw new ArgumentNullException(nameof(row));

        var id = row.Id?.OriginalValue?.ToString();
        if (id == null)
            throw new InvalidOperationException("Cannot find row id");

        return id;
    }
}
