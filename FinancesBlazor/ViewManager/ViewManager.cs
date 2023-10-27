using Finances.DependencyInjection;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinancesBlazor.ViewManager;

public partial class ViewManager : IInjectAsSingleton
{
    public readonly List<View> Views;
    private readonly ViewListParametersSerializer _serializer;
    private View _activeView;
    public View ActiveView { get => _activeView; set => _activeView = value; }

    public ViewManager(List<View> views, ViewListParametersSerializer serializer)
    {
        Views = views.OrderBy(v => v.Presentation?.NavMenuIndex).ToList();
        _serializer = serializer;
        _activeView = Views.FirstOrDefault() ?? throw new InvalidOperationException("No view found");
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
        if (vd.SortingColumnDataName != null)
        {
            ActiveView.Parameters.SortingColumnDataName = vd.SortingColumnDataName;
            ActiveView.Parameters.SortingDescending = vd.SortingDescending;
        }
    }

    private View? FindView(string? viewName)
    {
        if (string.IsNullOrWhiteSpace(viewName))
            return null;

        return Views.FirstOrDefault(v => v.Name == viewName);
    }
}

