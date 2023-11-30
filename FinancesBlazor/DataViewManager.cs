using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using Radzen;
using System;
using System.Collections.Specialized;
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
        if (DataViews.Count == 0)
            throw new InvalidOperationException("No view found");

        //Force redirect on first load, so query string appears in url and view is loaded.
        NameValueCollection qs = GetQueryString();
        if (qs["av"] == null)
        {
            qs["av"] = DataViews.First().Name;
            _activeView = DataViews.First();
        } 
        else
        {
            var activeView = FindView(qs["av"]);
            if (activeView == null)
            {
                qs["av"] = DataViews.First().Name;
                activeView = DataViews.First();
            }
            _activeView = activeView;
        }
        qs[_activeView.Name] = _activeView.Query.Serialize();
        _navigationManager.NavigateTo($"{GetUriWithoutQueryString()}?{SerializeQueryString(qs)}");
    }

    private async void _navigationManager_LocationChanged(object? sender, LocationChangedEventArgs e)
    {
        await LoadFromQueryString();
    }

    public async Task Save(DataView.DataView dataView)
    {
        var qs = GetQueryString();
        qs[dataView.Name] = dataView.Query.Serialize();
        await _jsRuntime.InvokeVoidAsync("ChangeUrl", $"{GetUriWithoutQueryString()}?{SerializeQueryString(qs)}");
        await dataView.Requery();
        ViewChanged?.Invoke(this, dataView);
    }

    private async Task LoadFromQueryString()
    {
        var qs = GetQueryString();
        var newActiveView = ActiveView;
        foreach (var key in qs.AllKeys)
        {
            if (key == "av")
            {
                newActiveView = FindView(qs[key]) ?? ActiveView;
                continue;
            }
            var view = FindView(key);
            if (view == null || qs[key] == null)
                continue;
            view.Query.Deserialize(qs[key]!);
            await view.Requery();
            ViewChanged?.Invoke(this, view);
        } 
        
        var activeViewChanged = ActiveView.Name != newActiveView.Name;
        ActiveView = newActiveView;
        if (activeViewChanged)
            ActiveViewChanged?.Invoke(this, ActiveView);
        
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

    private NameValueCollection GetQueryString()
    {
        var query = _navigationManager.ToAbsoluteUri(_navigationManager.Uri).Query;
        if (query.StartsWith("?"))
            query = query.Substring(1);
        if (string.IsNullOrWhiteSpace(query))
            return new NameValueCollection();

        return HttpUtility.ParseQueryString(query);
    }

    private string GetUriWithoutQueryString()
        => _navigationManager.ToAbsoluteUri(_navigationManager.Uri).GetLeftPart(UriPartial.Path);

    private string SerializeQueryString(NameValueCollection queryString) 
        => String.Join("&", queryString.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(queryString[a])));
}

