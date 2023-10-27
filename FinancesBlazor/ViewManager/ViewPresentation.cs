namespace FinancesBlazor.ViewManager;

public class ViewPresentation
{
    public int NavMenuIndex { get; }

    public string NavMenuIcon { get; }

    public string NavMenuTitle { get; }

    public ViewPresentation(int navMenuIndex, string navMenuIcon, string navMenuTitle)
    {
        NavMenuIndex = navMenuIndex;
        NavMenuIcon = navMenuIcon;
        NavMenuTitle = navMenuTitle;
    }
}
