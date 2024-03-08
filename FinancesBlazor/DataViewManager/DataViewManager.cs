using DataSource.Document;
using DataViews;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Radzen;
using System.Collections.Specialized;
using System.Web;
using FinancesBlazor.Components.Password;
using Microsoft.JSInterop;

namespace FinancesBlazor;

public class DataViewManager : IDisposable
{
    public readonly List<DataView> DataViews;
    private readonly NavigationManager _navigationManager;
    private readonly IDocumentManager _documentManager;
    private readonly DialogService _dialogService;
    private readonly IJSRuntime _js;
    private DataView? _activeView;
    public DataView? ActiveView { get => _activeView; }

    public event EventHandler<DataView>? ViewChanged;
    public event EventHandler<DataView>? ActiveViewChanged;
    private static string _documentPass = "";

    public SelectedData SelectedData { get; } = new SelectedData();

    public DataViewManager(NavigationManager navigationManager, List<DataView> dataViews, IDocumentManager documentManager, DialogService dialogService, IJSRuntime js)
    {
        _navigationManager = navigationManager;
        _documentManager = documentManager;
        _dialogService = dialogService;
        _js = js;
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

    public void Save(DataView? dataView)
    {
        if (dataView == null)
            throw new ArgumentNullException(nameof(dataView));

        var qs = GetQueryString();
        qs[dataView.Name] = dataView.Serialize();
        qs["sdi"] = string.Join(',', SelectedData.Records.Select(cr => $"{cr.Key}:{cr.Value.Name}"));
        qs["sdc"] = SelectedData.DetailsCollapsed ? "1" : "0";
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

        var newDetailsCollapsed = qs["sdc"] == "1";
        if (newDetailsCollapsed != SelectedData.DetailsCollapsed)
        {
            SelectedData.DetailsCollapsed = newDetailsCollapsed;
            SelectedData.InvokeDetailsCollapsedChanged();
        }

        var cr = qs["sdi"];
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


    public async Task SaveChanges(DataView? dataView, List<DataSource.DataRow>? rows)
    {
        if (dataView == null || rows == null)
            return;

     //   ViewChanged?.Invoke(this, dataView);
        await dataView.Save(rows);

        //Refresh lists that use updated details view.
        foreach(var dv in DataViews) {
            if (dv.GetDetailsDataViewName() == dataView.Name) //TODO: If more than 1 details view is used it needs to be checked too.
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
                _documentPass = await _dialogService.OpenAsync<PasswordInput>("Hasło");
            if (string.IsNullOrEmpty(_documentPass))
                return;
            _documentManager.DecompressDocument(fileName, _documentPass);
        }

        var files = _documentManager.GetDecompressedInfo(fileName).ToList();
        var url = "documents";
        if (files.Count == 1 && Path.GetExtension(files.First()).ToLowerInvariant() is ".pdf" or ".png" or ".jpg" or ".jpeg" or ".bmp")
            url = $"{url}/{files.First().Replace("\\", "/")}";
        else
            url = $"{url}/{Path.GetDirectoryName(files.First())}";
        url = $"{GetUriWithoutQueryString()}{url}";
        await _js.InvokeVoidAsync("openFile", url);
    }

    public async Task OpenGroup(string groupId)
    {
        var groups = DataViews.FirstOrDefault(dv => dv.Name == "gr") ?? throw new InvalidOperationException("Groups returned null");

        groups.Query.Reset();
        groups.Query.Filters.Add(groups.Columns.First(c => c.PrimaryDataColumnName == "GroupId"), new DataViewColumnFilter { StringValue = new List<string> { groupId } });
        await groups.Requery();

        foreach (var dataRow in groups.Result!.Rows)
        {
            var id = dataRow["RowId"].CurrentValue as string;
            var dataViewName = dataRow["DataViewName"].CurrentValue as string;
            var dataView = DataViews.FirstOrDefault(dv => dv.Name == dataViewName);
            if (id == null || dataView == null || SelectedData.Contains(dataRow))
                continue;

            SelectedData.Add(dataView, id!);
        }
        Save(ActiveView);

        //if (DataView == null || Row == null)
        //    return;

        //var detailsDataView = _dataViewManager.FindViewByName(DataView.GetDetailsDataViewName());

        //if (args)
        //    _dataViewManager.SelectedData.Add(detailsDataView, Row);
        //else
        //    _dataViewManager.SelectedData.Remove(Row);
        //_checked = args;
        //_dataViewManager.Save(DataView);
    }

    public async Task GroupSelectedData()
    {
        var allSelectedDataRows = SelectedData.Records
            .Select(r => r.Value.Result?.GetById(r.Key)) //Get data rows
            .Where(dr => dr?.SelectedInDetails == true); //that are checked

        //var selectedGroupIds = allSelectedDataRows
        //    .Select(dr => dr.GroupId?.CurrentValue as string) //get their groupId
        //    .Where(groupId => !string.IsNullOrWhiteSpace(groupId)) //exclude those without group
        //    .Distinct();

        ////Check if we are connecting to existing group or creating new one.
        //var groupId = string.Empty;
        //var count = selectedGroupIds.Count();

        ////if no groupId generate new
        //if (count == 0)
        //    groupId = Guid.NewGuid().ToString();
        ////if 1 groupId, reuse it (attach to existing group)
        //if (count == 1)
        //    groupId = selectedGroupIds.First();
        ////if more than 1 groupId throw error (cannot merge 2 groups)
        //if (count > 1)
        //    return;

        ////Create rows that need to be added into group dataSource.
        //var dataRowsToAdd = new List<DataRow>();
        //foreach (var selectedDataRow in allSelectedDataRows)
        //{
        //    if (selectedDataRow?.GroupId?.CurrentValue != null)
        //        continue; //No need to update records which are already in group.
        //    var groupDataRow = new DataRow();
        //    groupDataRow["Id"] = new DataValue(null, Guid.NewGuid().ToString());
        //    groupDataRow["GroupId"] = new DataValue(null, groupId);
        //    //Find view related to given detail
        //    groupDataRow["DataViewName"] = new DataValue(null, SelectedData.Records[selectedDataRow.Id.CurrentValue as string].Name);
        //    groupDataRow["RowId"] = new DataValue(null, selectedDataRow.Id.CurrentValue as string);
        //    groupDataRow["DocumentNumber"] = new DataValue(null, selectedDataRow.ContainsKey("Number") ? selectedDataRow["Number"].OriginalValue : null);
        //    dataRowsToAdd.Add(groupDataRow);
        //}
        //await SaveChanges(DataViews.FirstOrDefault(dv => dv.Name == "gr"), dataRowsToAdd);
    }
}

