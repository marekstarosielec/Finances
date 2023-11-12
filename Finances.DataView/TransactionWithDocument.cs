using DataView;
using Finances.DataSource;

namespace FinancesDataView;

public class TransactionWithDocument : IDataView
{
    private readonly DataSourceFactory _dataSourceFactory;
    private DataView.DataView? _dataView;

    public TransactionWithDocument(DataSourceFactory dataSourceFactory)
    {
        _dataSourceFactory = dataSourceFactory;
    }

    public DataView.DataView GetDataView()
    {
        if (_dataView != null)
            return _dataView;

        var presentation = new DataViewPresentation(50, "fa-solid fa-money-bill-transfer", "Tranzakcje");
        var columns = new List<DataViewColumn>
        {
            new DataViewColumnDate("Date", "Data", "d"),
            new DataViewColumnText("Account", "Konto", "a"),
            new DataViewColumnText("Category", "Kategoria", "ct"),
            //new PropertyInfoMoney("Amount", "Currency", "Kwota", "am"),
            new DataViewColumnText("Description", "Opis", "de"/*, TextFilterComponents.Default*/),
            new DataViewColumnText("Document.Company", "Firma dokumentu", "n")
        };

        _dataView = new ("t", "Tranzakcje", _dataSourceFactory.TransactionWithDocument, presentation)
        {
            Columns = new (columns)
        };

        _dataView.Query.Sorters.Add(columns.Single(c => c.ShortName == "d"), true);
        return _dataView;
    }
}