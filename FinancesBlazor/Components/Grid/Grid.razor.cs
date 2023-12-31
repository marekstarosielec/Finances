
using DataView;
using FinancesBlazor.Components.Grid.Filters;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor.Rendering;

namespace FinancesBlazor.Components.Grid;

public partial class Grid
{
    private Dictionary<DataViewColumn, ElementReference> _popupInvokers = new();
    private Dictionary<DataViewColumn, Popup> _popups = new();
    private Dictionary<DataViewColumn, DynamicFilter> _dynamicFilters = new();

    [Parameter]
    public DataView.DataView? DataView { get; set; }

    protected override void OnInitialized()
    {
        _dataViewManager.ViewChanged += ViewChanged;
    }

    private void ToggleFilter(DataViewColumn column)
    {
        _popups[column].ToggleAsync(_popupInvokers[column]);
    }

    // private async Task CloseFilters()
    // {
    //     foreach (var kvp in _popups)
    //         await kvp.Value.CloseAsync();
    // }

    private void LoadMore()
    {
        if (DataView == null)
            return;

        DataView.Query.PageSize += 100;
        _dataViewManager.Save(DataView);
    }

    private bool ColumnHeaderArrowHidden(DataViewColumn column, bool descending)
        => DataView?.Query.Sorters.ContainsKey(column) == false || DataView?.Query.Sorters[column] != descending;

    async void ViewChanged(object? sender, DataView.DataView e)
    {
        if (DataView != e)
            return;
        StateHasChanged();
        await DataView.Requery();
        StateHasChanged();
    }

    void IDisposable.Dispose()
    {
        _dataViewManager.ViewChanged -= ViewChanged;
    }
}
