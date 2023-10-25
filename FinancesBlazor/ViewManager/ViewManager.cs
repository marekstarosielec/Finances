using Finances.DependencyInjection;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinancesBlazor.ViewManager;

public partial class ViewManager : IInjectAsSingleton
{
    private readonly ViewsList _viewsList;
    private readonly ViewListParametersSerializer _serializer;
    private View _activeView;
    public View ActiveView { get => _activeView; set => _activeView = value; }

    public ViewManager(ViewsList viewsList, ViewListParametersSerializer serializer)
    {
        _viewsList = viewsList;
        _serializer = serializer;
        _activeView = _viewsList.Electricity;
    }

    public async Task SaveView(NavigationManager navigationManager, IJSRuntime js)
    {
        var vd = await _serializer.Serialize(ActiveView.Name, _activeView.Parameters);
        var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
        try
        {
            await js.InvokeVoidAsync("ChangeUrl", $"{uri.GetLeftPart(UriPartial.Path)}?{vd}");
        }
        catch (Exception ex) { }
    }

    public void LoadView(NavigationManager navigationManager)
    {
        var query = navigationManager.ToAbsoluteUri(navigationManager.Uri).Query;
        if (query.StartsWith("?"))
            query = query.Substring(1);
        if (string.IsNullOrWhiteSpace(query))
            return;
        var vd = _serializer.Deserialize(query);
        var activeView = FindView(vd.ActiveViewName);
        if (activeView == null)
            return;
        ActiveView = activeView;
        ActiveView.Parameters.SortingColumnDataName = vd.SortingColumnDataName;
        ActiveView.Parameters.SortingDescending = vd.SortingDescending;
    }

    private View? FindView(string? viewName)
    {
        if (string.IsNullOrWhiteSpace(viewName))
            return null;

        var properties = _viewsList.GetType().GetProperties(System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (var property in properties) {
            if (property.PropertyType != typeof(View))
                continue;

            var view = (View?) property.GetValue(_viewsList);
            if (view?.Name == viewName) return view;
        }

        return null;
    }
}

