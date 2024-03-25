using DataSource.Document;
using DataViews;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Radzen;
using System.Collections.Specialized;
using System.Web;
using FinancesBlazor.Components.Password;
using Microsoft.JSInterop;
using DataSource;
using FinancesBlazor.Extensions;

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
        NameValueCollection qs = GetNavigationManagerQueryString();
        if (qs["av"] == null || FindView(qs["av"]) == null)
            qs["av"] = DataViews.First(dv => dv.Presentation?.NavMenuIndex != null).Name;

        //Setup initial values
        DataViews.ForEach(dv =>
        {
            dv.SelectRecordFunc = SelectRecord;
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
        LoadFromQueryString(GetNavigationManagerQueryString());
    }

    public async Task RebuildView(DataView? dataView)
    {
        if (dataView == null)
            throw new ArgumentNullException(nameof(dataView));

        var qs = await GetJSQueryString();
        qs[dataView.Name] = dataView.Serialize();
        qs["sdi"] = string.Join(',', SelectedData.Records.Select(cr => $"{cr.Key}:{cr.Value.Name}"));
        qs["sdc"] = SelectedData.DetailsCollapsed ? "1" : "0";
        await _js.ChangeRouteWithoutReload($"{GetUriWithoutQueryString()}?{SerializeQueryString(qs)}");
        LoadFromQueryString(qs);
    }

    public void RemoveCache(DataView dataView)
    {
        dataView.RemoveCache();
        InvokeViewChanged(dataView);
    }

    public async Task ChangeActiveView(DataView dataView)
    {
        if (_activeView?.Name == dataView.Name)
            return;

        var qs = await GetJSQueryString();
        qs["av"] = dataView.Name;
        await _js.ChangeRouteWithoutReload($"{GetUriWithoutQueryString()}?{SerializeQueryString(qs)}");
        LoadFromQueryString(qs);
    }

    private void LoadFromQueryString(NameValueCollection qs)
    {
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

            if (dv.Serialize() == qs[dv.Name] && dv.Name != _activeView?.Name)
                continue;

            dv.Deserialize(qs[dv.Name]!);
            InvokeViewChanged(dv);
        }
    }

    private void InvokeViewChanged(DataView dv)
    {
        Console.WriteLine($"DataViewManager: Invoking view changed for {dv.Name}");
        ViewChanged?.Invoke(this, dv);
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

    private NameValueCollection GetNavigationManagerQueryString()
    {
        var query = _navigationManager.ToAbsoluteUri(_navigationManager.Uri).Query;
        if (query.StartsWith("?"))
            query = query.Substring(1);
        if (string.IsNullOrWhiteSpace(query))
            return new NameValueCollection();

        return HttpUtility.ParseQueryString(query);
    }

    private async Task<NameValueCollection> GetJSQueryString()
    {
        var query = await _js.GetQueryString();
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

        await dataView.Save(rows);
        dataView.RemoveCache();
        
        //Refresh lists that use updated details view.
        foreach (var dv in DataViews.Where(dv => dv.IsCacheInvalidated))
            InvokeViewChanged(dv);

        SelectedData.InvokeChanged();
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

        //Search for all rows with same groupId.
        groups.Query.Reset();
        groups.Query.Filters.Add(groups.Columns.First(c => c.PrimaryDataColumnName == GroupDataSource.GroupIdDataColumn.ColumnName), new DataViewColumnFilter { StringValue = new List<string> { groupId } });
        await groups.Requery();

        foreach (var dataRow in groups.Result!.Rows)
        {
            //Add each row into visible details.
            var id = dataRow[GroupDataSource.RowIdDataColumn.ColumnName].CurrentValue as string;
            var dataViewName = dataRow[GroupDataSource.DataViewNameDataColumn.ColumnName].CurrentValue as string;
            var dataView = DataViews.FirstOrDefault(dv => dv.Name == dataViewName);
            if (id == null || dataView == null || SelectedData.Contains(dataRow))
                continue;

            SelectedData.Add(dataView, id!);
        }
        await RebuildView(ActiveView);
    }

    public async Task GroupSelectedData()
    {
        var allSelectedDataRows = SelectedData.Records
            .Select(r => r.Value.Result?.GetById(r.Key)) //Get data rows
            .Where(dr => dr?.SelectedInDetails == true); //that are checked

        if (allSelectedDataRows.Count() < 2)
        {
            await Alert("Trzeba wybrac przynajmniej 2 elementy.");
            return;
        }

        var selectedGroupIds = allSelectedDataRows
            .Select(dr => dr?.GroupId) //get their groupId
            .Where(groupId => !string.IsNullOrWhiteSpace(groupId)) //exclude those without group
            .Distinct();

        //Check if we are connecting to existing group or creating new one.
        var groupId = string.Empty;
        var count = selectedGroupIds.Count();

        //if no groupId generate new
        if (count == 0)
            groupId = Guid.NewGuid().ToString();
        //if 1 groupId, reuse it (attach to existing group)
        if (count == 1)
            groupId = selectedGroupIds.First();
        //if more than 1 groupId throw error (cannot merge 2 groups)
        if (count > 1)
        {
            await Alert("Wybrane elementy już znajdują się w różnych grupach.");
            return;
        }


        //Create rows that need to be added into group dataSource.
        var dataRowsToAdd = new List<DataRow>();
        foreach (var selectedDataRow in allSelectedDataRows)
        {
            if (selectedDataRow?.GroupId != null)
                continue; //No need to update records which are already in group.
            var groupDataRow = new DataRow();
            groupDataRow["Id"] = new DataValue(null, Guid.NewGuid().ToString());
            groupDataRow[GroupDataSource.GroupIdDataColumn.ColumnName] = new DataValue(null, groupId);
            //Find view related to given detail
            groupDataRow[GroupDataSource.DataViewNameDataColumn.ColumnName] = new DataValue(null, SelectedData.Records[selectedDataRow.Id.CurrentValue as string].Name);
            groupDataRow[GroupDataSource.RowIdDataColumn.ColumnName] = new DataValue(null, selectedDataRow.Id.CurrentValue as string);
            groupDataRow["DocumentNumber"] = new DataValue(null, selectedDataRow.ContainsKey("Number") ? selectedDataRow["Number"].OriginalValue : null);
            dataRowsToAdd.Add(groupDataRow);
        }

        if (dataRowsToAdd.Count > 0) 
            await SaveChanges(DataViews.FirstOrDefault(dv => dv.Name == "gr"), dataRowsToAdd);

        foreach (var item in allSelectedDataRows)
            item!.SelectedInDetails = false;
        SelectedData.InvokeChanged();
    }

    public async Task Alert(string message)
    {
        await _dialogService.Alert(message, string.Empty, options: new AlertOptions { CloseDialogOnEsc = true, CloseDialogOnOverlayClick = true, OkButtonText = "Ok", ShowClose = false, ShowTitle = false });
    }

    private async Task SelectRecord(DataView dataView, DataRow row, DataViewColumn column)
    {

    }
}

