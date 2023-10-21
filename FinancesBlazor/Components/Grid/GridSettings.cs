namespace FinancesBlazor.Components.Grid
{
    public class GridSettings
    {
        public string? SortingColumnDataName { get; set; }

        public bool SortingDescending { get; set; }

        public GridColumn[] Columns { get; set; } = new GridColumn[0];
    }
}
