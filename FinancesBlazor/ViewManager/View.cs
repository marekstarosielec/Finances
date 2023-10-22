using Finances.DataAccess;
using FinancesBlazor.Components.Grid;

namespace FinancesBlazor.ViewManager;

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
