using System.Collections.ObjectModel;

namespace FinancesBlazor.ViewManager;

public class ViewListParameters
{
    public string? SortingColumnDataName { get; set; }

    public bool SortingDescending { get; set; }

    public ReadOnlyCollection<Column>? Columns { get; init; }
}
