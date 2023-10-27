using System.Collections.ObjectModel;

namespace FinancesBlazor.ViewManager;

public class ViewListParameters
{
    public string? SortingColumnDataName { get; set; }

    public bool SortingDescending { get; set; }

    public ReadOnlyCollection<Column> Columns { get; init; } = new ReadOnlyCollection<Column>(new List<Column>());

    public Dictionary<Column, FilterValue> Filters { get; } = new Dictionary<Column, FilterValue>();

    public int MaximumNumberOfRecords { get; set; } = 100;
}
