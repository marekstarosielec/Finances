namespace FinancesBlazor.Components.Grid
{
    public class GridSettings
    {
        public string Name {  get; set; }

        public string? SortingColumnDataName { get; set; }

        public bool SortingDescending { get; set; }

        public GridColumn[] Columns { get; set; } = Array.Empty<GridColumn>();

        public GridSettings(string name)
        {
            Name = name;
        }
    }
}
