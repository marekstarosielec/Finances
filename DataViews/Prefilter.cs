namespace DataViews;

public class Prefilter
{
    public string Name { get; set; }

    public string Title { get; set; }

    public DataViewColumn Column { get; set; }

    public DataViewColumnFilter ColumnFilter { get; set; }

    public bool Applied { get; set; }

    public bool DefaultApplied { get; }

    public Prefilter(string name, string title, DataViewColumn column, DataViewColumnFilter columnFilter, bool applied = false)
    {
        Name = name;
        Title = title;
        Column = column;
        ColumnFilter = columnFilter;
        Applied = applied;
        DefaultApplied = applied;
    }

}
