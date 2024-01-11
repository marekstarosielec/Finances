using DataSource.Document;
using DataViews;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Radzen;
using System.Collections.Specialized;
using System.Web;
using FinancesBlazor.Components.Password;

namespace FinancesBlazor;

public class DataViewManager : IDisposable
{
    public readonly List<DataView> DataViews;
    private readonly NavigationManager _navigationManager;
    private readonly IDocumentManager _documentManager;
    private readonly DialogService _dialogService;
    private DataView? _activeView;
    public DataView? ActiveView { get => _activeView; }

    public event EventHandler<DataView>? ViewChanged;
    public event EventHandler<DataView>? ActiveViewChanged;
    private static string _documentPass = "";

    public SelectedData SelectedData { get; } = new SelectedData();

    public DataViewManager(NavigationManager navigationManager, List<DataView> dataViews, IDocumentManager documentManager, DialogService dialogService)
    {
        _navigationManager = navigationManager;
        _documentManager = documentManager;
        _dialogService = dialogService;
        _navigationManager.LocationChanged += _navigationManager_LocationChanged;
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

    public void Save(DataView dataView)
    {
        var qs = GetQueryString();
        qs[dataView.Name] = dataView.Serialize();
        qs["cr"] = string.Join(',', SelectedData.Ids.Select(cr => $"{cr.Key}:{cr.Value.Name}"));
        qs["sd"] = SelectedData.DetailsCollapsed ? "1" : "0";
        _navigationManager.NavigateTo($"{GetUriWithoutQueryString()}?{SerializeQueryString(qs)}");
    }

    public void RemoveCache(DataView dataView)
    {
        dataView.RemoveCache();
        ViewChanged?.Invoke(this, dataView);
    }

    public void ChangeActiveView(DataView dataView)
    {
        if (_activeView?.Name == dataView.Name)
            return;

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

        var newDetailsCollapsed = qs["sd"] == "1";
        if (newDetailsCollapsed != SelectedData.DetailsCollapsed)
        {
            SelectedData.DetailsCollapsed = newDetailsCollapsed;
            SelectedData.InvokeDetailsCollapsedChanged();
        }

        var cr = qs["cr"];
        SelectedData.Clear();
        var checkedRecords = cr?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new ();
        foreach (var checkedRecord in checkedRecords)
        {
            var pos = checkedRecord.IndexOf(':');
            if (pos == -1)
                continue;
            var checkedRecordId = checkedRecord.Substring(0, pos);
            var checkedRecordDataViewId = checkedRecord.Substring(pos + 1);
            var checkedRecordDataView = DataViews.FirstOrDefault(dv => dv.Name == checkedRecordDataViewId);
            if (checkedRecordDataView == null)
                continue;

            SelectedData.Add(checkedRecordDataView, checkedRecordId);
        }
        SelectedData.InvokeChanged();

        foreach (var dv in DataViews)
        {
            if (qs[dv.Name] == null)
                continue;

            dv.Deserialize(qs[dv.Name]!);
            ViewChanged?.Invoke(this, dv);
        }
    }

    private DataView? FindView(string? viewName)
    {
        if (string.IsNullOrWhiteSpace(viewName))
            return null;

        return DataViews.FirstOrDefault(v => v.Name == viewName);
    }

    public DataView FindViewByName(string? viewName) => DataViews.FirstOrDefault(v => v.Name == viewName) ?? throw new InvalidOperationException($"Cannot find view {viewName}");

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
        => string.Join("&", queryString.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(queryString[a])));


    public async Task SaveChanges(DataView? dataView, DataSource.DataRow? row)
    {
        if (dataView == null || row == null)
            return;

     //   ViewChanged?.Invoke(this, dataView);
        await dataView.Save(row);
        foreach(var dv in DataViews) {
            if (dv.GetDetailsDataViewName() == dataView.Name) //TODO: If more than 1 details view is used it needs to be cehcked too.
            {
                dataView.RemoveCache();
                ViewChanged?.Invoke(this, dv);
            }
        }
        ViewChanged?.Invoke(this, dataView);
    }

    public async Task OpenDocument(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return;

        if (!_documentManager.IsDocumentDecompressed(fileName))
        {
            if (string.IsNullOrEmpty(_documentPass))
            {
                _documentPass = await _dialogService.OpenAsync<PasswordInput>("Hasło");
            }
            if (string.IsNullOrEmpty(_documentPass))
                return;
            _documentManager.DecompressDocument(fileName, _documentPass);
        }
       // DocumentManager.
    }
}

