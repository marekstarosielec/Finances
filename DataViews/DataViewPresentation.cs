namespace DataViews;

public class DataViewPresentation
{
    public int? NavMenuIndex { get; }

    public string NavMenuIcon { get; }

    public string NavMenuTitle { get; }

    public DataViewPresentation(int? navMenuIndex, string navMenuIcon, string navMenuTitle)
    {
        NavMenuIndex = navMenuIndex;
        NavMenuIcon = navMenuIcon;
        NavMenuTitle = navMenuTitle;
    }

    public DataViewPresentation Clone() =>
        new DataViewPresentation(NavMenuIndex, NavMenuIcon, NavMenuTitle);
}
