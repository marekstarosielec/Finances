using DataSource;
using System.Collections.ObjectModel;

namespace DataView;

public class DataView
{
    public string Name { get; }

    public string Title { get; set; }

    public string? SortingColumnPropertyName { get; set; }

    public bool SortingDescending { get; set; }

    public ReadOnlyCollection<DataViewColumn> Columns { get; init; } = new(new List<DataViewColumn>());

    public Dictionary<DataViewColumn, DataViewColumnFilter> Filters { get; } = new Dictionary<DataViewColumn, DataViewColumnFilter>();

    public int MaximumNumberOfRecords { get; set; } = 100;

    public IDataSource DataSource { get; }

    public DataViewPresentation? Presentation { get; }

    public DataView(string name, string title, IDataSource dataSource, DataViewPresentation? presentation = null)
    {
        Name = name;
        Title = title;
        DataSource = dataSource;
        Presentation = presentation;
    }
}
