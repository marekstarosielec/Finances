using Finances.DependencyInjection;
using FinancesBlazor.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace FinancesBlazor.ViewManager;

public class ViewManager : IDisposable
{
    public readonly List<View> Views;
    private readonly NavigationManager _navigationManager;
    private readonly IJSRuntime _jsRuntime;
    private readonly ViewSerializer _serializer;
    private View _activeView;
    public View ActiveView { get => _activeView; set => _activeView = value; }

    public event EventHandler<View>? ViewChanged;

    public ViewManager(NavigationManager navigationManager, IJSRuntime jsRuntime, List<View> views, ViewSerializer serializer)
    {
        _navigationManager = navigationManager;
        _navigationManager.LocationChanged += _navigationManager_LocationChanged;
        _jsRuntime = jsRuntime;
        Views = views.OrderBy(v => v.Presentation?.NavMenuIndex).ToList();
        _serializer = serializer;
        
        //Force redirect on first load, so query string appears in url and view is loaded.
        _activeView = Views.FirstOrDefault() ?? throw new InvalidOperationException("No view found");
        var vd = _serializer.Serialize(_activeView.Name, _activeView);
        var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri).GetLeftPart(UriPartial.Path);
        _navigationManager.NavigateTo($"{uri}?{vd}");
    }

    private async void _navigationManager_LocationChanged(object? sender, LocationChangedEventArgs e)
    {
        await LoadFromQueryString();
    }

    public async Task Save(View view)
    {
        if (view == ActiveView)
        {
            var vd = _serializer.Serialize(_activeView.Name, _activeView);
            var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
            try
            {
                await _jsRuntime.InvokeVoidAsync("ChangeUrl", $"{uri.GetLeftPart(UriPartial.Path)}?{vd}");
            }
            catch (Exception ex) { }
        }
        await view.Service.Reload();
        ViewChanged?.Invoke(this, view);
    }

    private async Task LoadFromQueryString()
    {
        var query = _navigationManager.ToAbsoluteUri(_navigationManager.Uri).Query;
        if (query.StartsWith("?"))
            query = query.Substring(1);
        if (string.IsNullOrWhiteSpace(query))
            return;
        var vd = _serializer.Deserialize(query);
        var activeView = FindView(vd.ActiveViewName);
        if (activeView == null)
            return;
        ActiveView = activeView;
        if (vd.SortingColumnDataName != null)
        {
            ActiveView.SortingColumnPropertyName = vd.SortingColumnDataName;
            ActiveView.SortingDescending = vd.SortingDescending;
        }
        await ActiveView.Service.Reload();
        ViewChanged?.Invoke(this, ActiveView);
    }

    private View? FindView(string? viewName)
    {
        if (string.IsNullOrWhiteSpace(viewName))
            return null;

        return Views.FirstOrDefault(v => v.Name == viewName);
    }

    public void Dispose()
    {
        _navigationManager.LocationChanged -= _navigationManager_LocationChanged;
    }
}

