using DataView;
using Finances.DataSource;

namespace FinancesDataView;

public class TransactionDetails : IDataView
{
    private readonly DataSourceFactory _dataSourceFactory;
    private DataView.DataView? _dataView;

    public TransactionDetails(DataSourceFactory dataSourceFactory)
    {
        _dataSourceFactory = dataSourceFactory;
    }

    public DataView.DataView GetDataView()
    {
        if (_dataView != null)
            return _dataView;

        var columns = new List<DataViewColumn>
        {
            //new DataViewColumnDate("Date", "Data", "d"),
            //new DataViewColumnText("Account", "Konto", "a"),
            //new DataViewColumnText("Category", "Kategoria", "ct"),
            //new DataViewColumnAmount("Amount", "Currency", "Kwota", "am"),
            new DataViewColumnText("Description", "Opis", "de", numberOfLinesInDetails: 3)
        };

        _dataView = new("td", "Szczegóły tranzakcji", _dataSourceFactory.Transaction, new(columns));
        return _dataView;
    }
}
