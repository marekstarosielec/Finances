using FinancesBlazor.Components.Details;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Radzen;
using System.Collections.Specialized;
using System.Data;
using System.Web;

namespace FinancesBlazor;

public class DataViewManager : IDisposable
{
    public readonly List<DataView.DataView> DataViews;
    private readonly NavigationManager _navigationManager;
    private readonly DialogService _dialogService;
    private DataView.DataView? _activeView;
    public DataView.DataView? ActiveView { get => _activeView; }

    public event EventHandler<DataView.DataView>? ViewChanged;
    public event EventHandler<DataView.DataView>? ActiveViewChanged;

    public DataViewManager(NavigationManager navigationManager, List<DataView.DataView> dataViews, DialogService dialogService)
    {
        _navigationManager = navigationManager;
        _navigationManager.LocationChanged += _navigationManager_LocationChanged;
        _dialogService = dialogService;
        DataViews = dataViews.OrderBy(v => v.Presentation?.NavMenuIndex).ToList();
        if (DataViews.Count == 0)
            throw new InvalidOperationException("No view found");
        
        //Determine initial view.
        NameValueCollection qs = GetQueryString();
        if (qs["av"] == null || FindView(qs["av"]) == null)
            qs["av"] = DataViews.First(dv => dv.Presentation != null).Name;

        //Setup initial values
        DataViews.ForEach(dv =>
        {
            var currentview = qs[dv.Name];
            if (!string.IsNullOrWhiteSpace(currentview))
                dv.Deserialize(currentview);
            else
                dv.Query.Reset();
            qs[dv.Name] = dv.Serialize();
        });
        _navigationManager.NavigateTo($"{GetUriWithoutQueryString()}?{SerializeQueryString(qs)}");
    }

    private void _navigationManager_LocationChanged(object? sender, LocationChangedEventArgs e)
    {
        LoadFromQueryString();
    }

    public void Save(DataView.DataView dataView)
    {
        var qs = GetQueryString();
        qs[dataView.Name] = dataView.Serialize();
        _navigationManager.NavigateTo($"{GetUriWithoutQueryString()}?{SerializeQueryString(qs)}");
    }

    public void RemoveCache(DataView.DataView dataView)
    {
        dataView.RemoveCache();
        ViewChanged?.Invoke(this, dataView);
    }

    public void ChangeActiveView(DataView.DataView dataView)
    {
        if (_activeView?.Name == dataView.Name)
            return;
        _dialogService.CloseSide();

        var qs = GetQueryString();
        qs["av"] = dataView.Name;
        _navigationManager.NavigateTo($"{GetUriWithoutQueryString()}?{SerializeQueryString(qs)}");
    }

    private void LoadFromQueryString()
    {
        var qs = GetQueryString();
        if (!string.Equals(qs["av"],ActiveView?.Name, StringComparison.CurrentCultureIgnoreCase))
        {
            var newActiveView = DataViews.FirstOrDefault(dv => string.Equals(qs["av"], dv.Name, StringComparison.CurrentCultureIgnoreCase));
            if (newActiveView != null)
            {
                _activeView = newActiveView;
                ActiveViewChanged?.Invoke(this, _activeView);
            }
        }

        if (_activeView?.SelectedRecordId != null)
            OpenSideDialog(true);
        if (_activeView?.CheckedRecordsCount > 0)
            OpenSideDialog(false);

        foreach (var dv in DataViews)
        {
            if (qs[dv.Name] == null)
                continue;

            dv.Deserialize(qs[dv.Name]!);
            ViewChanged?.Invoke(this, dv);
        }
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

    public async Task OpenSideDialog(bool singleRecord)
    {
        if (ActiveView == null)
            return;
        var width = singleRecord ? 300 : Math.Min(3, ActiveView.CheckedRecordsCount) * 300;

        await _dialogService.OpenSideAsync<Details>(string.Empty,
            parameters: new Dictionary<string, object>() { 
                { "DataView", ActiveView } 
            },
            options: new SideDialogOptions { 
                CloseDialogOnOverlayClick = false, 
                Position = DialogPosition.Right, 
                Width = $"{width}px",
                ShowMask = false, 
                ShowTitle = false
                });
    }
}

