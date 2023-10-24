using Finances.DataAccess;

namespace FinancesBlazor.ViewManager;

public class View
{
    public string Name { get; }

    public string Title { get; set; }

    public ViewListParameters Parameters { get; set; } = new ViewListParameters();

    public BaseListService Service { get; }

    public View(string name, string title, BaseListService service)
    {
        Name = name;
        Title = title;
        Service = service;
    }
}
