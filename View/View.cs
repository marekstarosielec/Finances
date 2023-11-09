using DataSource;
using System.Collections.ObjectModel;

namespace View;

public class View
{
    public string Name { get; }

    public string Title { get; set; }

    public string? SortingColumnPropertyName { get; set; }

    public bool SortingDescending { get; set; }

    public ReadOnlyCollection<ViewColumn> Columns { get; init; } = new(new List<ViewColumn>());

    public Dictionary<ViewColumn, ViewColumnFilter> Filters { get; } = new Dictionary<ViewColumn, ViewColumnFilter>();

    public int MaximumNumberOfRecords { get; set; } = 100;

    public IDataSource DataSource { get; }

    public ViewPresentation? Presentation { get; }

    public View(string name, string title, IDataSource dataSource, ViewPresentation? presentation = null)
    {
        Name = name;
        Title = title;
        DataSource = dataSource;
        Presentation = presentation;
    }
}
