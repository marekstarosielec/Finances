using Finances.DataAccess;
using FinancesBlazor.PropertyInfo;
using System.Collections.ObjectModel;

namespace FinancesBlazor.ViewManager;

public class View
{
    public string Name { get; }

    public string Title { get; set; }

    public string? SortingColumnPropertyName { get; set; }

    public bool SortingDescending { get; set; }

    public ReadOnlyCollection<PropertyInfoBase> Properties { get; init; } = new(new List<PropertyInfoBase>());

    public Dictionary<PropertyInfoBase, FilterInfoBase> Filters { get; } = new Dictionary<PropertyInfoBase, FilterInfoBase>();

    public int MaximumNumberOfRecords { get; set; } = 100;

    public BaseListService Service { get; }

    public ViewPresentation? Presentation { get; }

    public View(string name, string title, BaseListService service, ViewPresentation? presentation = null)
    {
        Name = name;
        Title = title;
        Service = service;
        Service.View = this;
        Presentation = presentation;
    }
}
