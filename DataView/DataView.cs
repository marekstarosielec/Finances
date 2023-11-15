using DataSource;
using System.Collections.ObjectModel;

namespace DataView;

public class DataView
{
    private IDataSource _dataSource;
    private DataQuery _query = new DataQuery();

    public string Name { get; }

    public string Title { get; set; }

    public ReadOnlyCollection<DataViewColumn> Columns { get; }

    public DataViewPresentation? Presentation { get; }

    public DataViewQuery Query { get; init; }

    public DataQueryResult? Result { get; private set; }

    public async Task Requery()
    {
        Query.Apply();
        Result = await _dataSource.ExecuteQuery(_query);
    }

    public DataView(string name, string title, IDataSource dataSource, ReadOnlyCollection<DataViewColumn> columns, DataViewPresentation? presentation = null)
    {
        Name = name;
        Title = title;
        _dataSource = dataSource;
        Presentation = presentation;
        Columns = columns;
        Query = new DataViewQuery(_query, _dataSource, Columns);

    }
}
