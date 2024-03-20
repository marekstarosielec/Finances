using DataSource;
using DataViews;
using Finances.DataSource;

namespace FinancesDataView;

public class TransactionAccountDetails : IDataView
{
    private readonly DataSourceFactory _dataSourceFactory;
    private DataView? _dataView;

    public TransactionAccountDetails(DataSourceFactory dataSourceFactory)
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
            new DataViewColumnText("Title", "Nazwa", "t"),
            new DataViewColumnText("Currency", "Waluta", "c")
        };

        _dataView = new("tad", "Szczegóły konta", _dataSourceFactory.TransactionAccount, new(columns));
        return _dataView;
    }
}
