using System.Collections.ObjectModel;
using FinancesBlazor.Components.Grid;

namespace FinancesBlazor.ViewManager
{
    public class ViewListParameters
    {
        public string? SortingColumnDataName { get; set; }

        public bool SortingDescending { get; set; }

        public ReadOnlyCollection<GridColumn>? Columns { get; init; }
    }
}
