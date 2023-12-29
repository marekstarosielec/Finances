using DataSource;
using DataView;
using FinancesBlazor.Components.Details;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Radzen;
using System.Collections.ObjectModel;
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

    private Dictionary<string, DataView.DataView> _checkedRecords = new Dictionary<string, DataView.DataView>();

    public ReadOnlyDictionary<string, DataView.DataView> CheckedRecords => new ReadOnlyDictionary<string, DataView.DataView>(_checkedRecords);

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
        qs["cr"] = string.Join(',', _checkedRecords.Select(cr => $"{cr.Key}:{cr.Value.Name}"));
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

        _checkedRecords.Clear();
        var cr = qs["cr"];
        if (cr != null)

        {
            var checkedRecords = cr.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var checkedRecord in checkedRecords)
            {
                var pos = checkedRecord.IndexOf(':');
                if (pos == -1)
                    continue;
                var checkedRecordId = checkedRecord.Substring(0, pos);
                var checkedRecordDataViewId = checkedRecord.Substring(pos+1);
                var checkedRecordDataView = DataViews.FirstOrDefault(dv => dv.Name == checkedRecordDataViewId);
                if (checkedRecordDataView == null)
                    continue;

                _checkedRecords[checkedRecordId] = checkedRecordDataView;
            }
        }

        if (_checkedRecords.Count > 0)
            OpenSideDialog();

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

    public async Task OpenSideDialog()
    {
        if (ActiveView == null)
            return;
        var width = Math.Min(DetailSettings.MaximumNumberOfDetails, CheckedRecords.Count) * DetailSettings.DetailsWidth;

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

    private string GetRowId(DataView.DataView dataView, DataSource.DataRow? row)
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

    public void CheckRecord(DataView.DataView dataView, DataSource.DataRow? row)
    {
        var detailsView = DataViews.FirstOrDefault(dv => dv.Name == dataView.GetDetailsDataViewName());
        if (detailsView == null)
            return;

        _checkedRecords.Add(GetRowId(dataView, row), detailsView);
    }

    public void UncheckRecord(DataView.DataView dataView, DataSource.DataRow? row)
    {
        _checkedRecords.Remove(GetRowId(dataView, row));
    }

    public void UncheckRecords()
    {
        _checkedRecords.Clear();
    }

    public bool RecordIsChecked(DataView.DataView dataView, DataSource.DataRow? row) => _checkedRecords.ContainsKey(GetRowId(dataView, row));


    public async Task SaveChanges(DataView.DataView? dataView, DataSource.DataRow? row)
    {
        if (dataView == null || row == null)
            return;

        ViewChanged?.Invoke(this, dataView);
        await dataView.Save(row);
        ViewChanged?.Invoke(this, dataView);
        foreach(var dv in DataViews) {
            if (dv.GetDetailsDataViewName() == dataView.Name) //TODO: If more than 1 details view is used it needs to be cehcked too.
            {
                ViewChanged?.Invoke(this, dv);
                dataView.RemoveCache();
                ViewChanged?.Invoke(this, dv);
            }
        }
    }
}

