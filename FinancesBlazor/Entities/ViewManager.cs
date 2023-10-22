using Finances.DataAccess;
using Finances.DependencyInjection;
using FinancesBlazor.Components.Grid;

namespace FinancesBlazor.Entities;

public partial class ViewManager : IInjectAsSingleton
{
    private readonly ViewsList _viewsList;
    private View _activeView;

    public View ActiveView { get => _activeView; set => _activeView = value; }

    public ViewManager(ViewsList viewsList)
    {
        _viewsList = viewsList;
        _activeView = _viewsList.Electricity;
    }
}

public class View
{
    public string Name { get; }

    public string Title { get; set; }

    public GridSettings? MainGridSettings { get; set; } 

    public BaseListService Service { get; }

    public View(string name, string title, BaseListService service)
    {
        Name = name;
        Title = title;
        Service = service;
    }
}

public partial class ViewsList : IInjectAsSingleton
{
    private static IConfiguration _configuration;

    public ViewsList(IConfiguration configuration)
    {
        _configuration = configuration;
    }
}