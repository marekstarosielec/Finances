using DataViews;
using Finances.DataSource;

namespace FinancesDataView;

public class TransactionAccountReferenceList : IDataView
{
    private readonly DataSourceFactory _dataSourceFactory;
    private DataView? _dataView;

    public TransactionAccountReferenceList(DataSourceFactory dataSourceFactory)
    {
        _dataSourceFactory = dataSourceFactory;
    }

    public DataView GetDataView()
    {
        if (_dataView != null)
            return _dataView;

        _dataView = new TransactionAccountMainList(_dataSourceFactory).GetDataView().Clone("tar");
        _dataView.Presentation = null;
        foreach (var column in _dataView.Columns)
            column.Visible = column.ShortName == "t";

        return _dataView;
    }
}