using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System.Web;

namespace FinancesBlazor;

public class DataViewManager : IDisposable
{
    public readonly List<DataView.DataView> DataViews;
    private readonly NavigationManager _navigationManager;
    private readonly IJSRuntime _jsRuntime;
    private DataView.DataView _activeView;
    public DataView.DataView ActiveView { get => _activeView; set => _activeView = value; }

    public event EventHandler<DataView.DataView>? ViewChanged;
    public event EventHandler<DataView.DataView>? ActiveViewChanged;

    public DataViewManager(NavigationManager navigationManager, IJSRuntime jsRuntime, List<DataView.DataView> dataViews)
    {
        _navigationManager = navigationManager;
        _navigationManager.LocationChanged += _navigationManager_LocationChanged;
        _jsRuntime = jsRuntime;
        DataViews = dataViews.OrderBy(v => v.Presentation?.NavMenuIndex).ToList();
      
        //Force redirect on first load, so query string appears in url and view is loaded.
        _activeView = DataViews.FirstOrDefault() ?? throw new InvalidOperationException("No view found");
        var vd = _activeView.Query.Serialize();
        var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri).GetLeftPart(UriPartial.Path);
        _navigationManager.NavigateTo($"{uri}?av={_activeView.Name}&{_activeView.Name}={HttpUtility.UrlEncode(vd)}");
    }

    private async void _navigationManager_LocationChanged(object? sender, LocationChangedEventArgs e)
    {
        await LoadFromQueryString();
    }

    public async Task Save(DataView.DataView dataView)
    {
        //if (dataView == ActiveView)
        //{
        //    var vd = dataView.Serialize();
        //    var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
        //    try
        //    {
        //        await _jsRuntime.InvokeVoidAsync("ChangeUrl", $"{uri.GetLeftPart(UriPartial.Path)}?{vd}");
        //    }
        //    catch (Exception ex) { }
        //}
        //await dataView.Requery();
        //ViewChanged?.Invoke(this, dataView);
    }

    private async Task LoadFromQueryString()
    {
        var query = _navigationManager.ToAbsoluteUri(_navigationManager.Uri).Query;
        if (query.StartsWith("?"))
            query = query.Substring(1);
        if (string.IsNullOrWhiteSpace(query))
            return;
        var activeView = HttpUtility.ParseQueryString(query).GetValues("av")?.FirstOrDefault();
        //Load all views found in query.
        //Switch active view if needed.
        
        //var activeView = FindView(vd);
        //if (activeView == null)
        //    return;
        //var activeViewChanged = ActiveView.Name != vd;
        //ActiveView = activeView;


        //if (vd.SortingColumnDataName != null)
        //{
        //    ActiveView.SortingColumnPropertyName = vd.SortingColumnDataName;
        //    ActiveView.SortingDescending = vd.SortingDescending;
        //}

        //ActiveView.Filters.Clear();
        //foreach (var filter in vd.Filters)
        //    ActiveView.Filters.Add(ActiveView.Columns.Single(p => p.ShortName == filter.Key), filter.Value);

        //await ActiveView.Requery();
        //if (activeViewChanged)
        //    ActiveViewChanged?.Invoke(this, ActiveView);
        //ViewChanged?.Invoke(this, ActiveView);
    }

    private DataView.DataView? FindView(string? viewName)
    {
        if (string.IsNullOrWhiteSpace(viewName))
            return null;

        return DataViews.FirstOrDefault(v => v.Name == viewName);
    }

    public void Dispose()
    {
        _navigationManager.LocationChanged -= _navigationManager_LocationChanged;
    }
}

