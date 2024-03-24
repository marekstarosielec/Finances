using DataSource;
using DataViews;
using Finances.DataSource;

namespace FinancesDataView;

/// <summary>
/// This cannot be removed, even if it is not visible, since dataViewManager uses it to manage group items.
/// </summary>
public class GroupView : IDataView
{
    private DataView? _dataView;

    public DataSourceFactory _dataSourceFactory { get; }

    public GroupView(DataSourceFactory dataSourceFactory)
    {
        _dataSourceFactory = dataSourceFactory;
    }
    
    public DataView GetDataView()
    {
        if (_dataView != null)
            return _dataView;

        var presentation = new DataViewPresentation { NavMenuIndex = 1000, NavMenuIcon = "fa-regular fa-object-group", NavMenuTitle = "Grupy" };
        var columns = new List<DataViewColumn>
        {
            new DataViewColumnText("Id", "Id", "id", visible: false),
            new DataViewColumnText("GroupId", "Grupa", "g"),
            new DataViewColumnText("DataViewName", "Widok", "v"),
            new DataViewColumnText("RowId", "Id wiersza", "rid"),
            new DataViewColumnDocumentLink("FileLink", "Plik", "f")
        };

        _dataView = new("gr", "Grupy", GroupDataSource.Instance, new(columns), presentation);
        return _dataView;
    }
}
