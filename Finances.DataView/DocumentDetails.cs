using DataViews;
using Finances.DataSource;

namespace FinancesDataView;

public class DocumentDetails : IDataView
{
    private readonly DataSourceFactory _dataSourceFactory;
    private DataView? _dataView;

    public DocumentDetails(DataSourceFactory dataSourceFactory)
    {
        _dataSourceFactory = dataSourceFactory;
    }

    public DataView GetDataView()
    {
        if (_dataView != null)
            return _dataView;

        var columns = new List<DataViewColumn>
        {
            new DataViewColumnText("Id", "Id", "id", visible: false),
            new DataViewColumnDate("Date", "Data", "d")
        };

        _dataView = new("dd", "Szczegóły dokumentu", _dataSourceFactory.Document, new(columns));
        return _dataView;
    }
}
