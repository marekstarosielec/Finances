namespace DataViews;

public class DataViewPresentation
{
    public int? NavMenuIndex { get; set; }

    public string? NavMenuIcon { get; set; }

    public string? NavMenuTitle { get; set; }

    public bool ShowSelectionCheckboxesInList { get; set; } = true;

    public bool ShowToolbar { get; set; } = true;

    public bool ShowHeaders { get; set; } = true;

    //public DataViewPresentation Clone() =>
    //    new DataViewPresentation(NavMenuIndex, NavMenuIcon, NavMenuTitle);
}
