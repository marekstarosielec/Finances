using DataViews;
using Finances.DataSource;

namespace FinancesDataView;

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

        var presentation = new DataViewPresentation(1000, "fa-regular fa-object-group", "Grupy");
        var columns = new List<DataViewColumn>
        {
            new DataViewColumnText("Id", "Id", "id", visible: false),
            new DataViewColumnText("GroupId", "Grupa", "g"),
            new DataViewColumnText("DataViewName", "Widok", "v"),
            new DataViewColumnDate("RowId", "Id wiersza", "rid")
        };

        _dataView = new("gr", "Grupy", _dataSourceFactory.Group, new(columns), presentation);
        return _dataView;
    }
}
