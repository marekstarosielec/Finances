using DataView;
using Finances.DataSource;

namespace Finances.DataView;

public class TransactionWithDocument : IDataView
{
    private readonly DataSourceFactory _dataSourceFactory;
    private global::DataView.DataView? _dataView;

    public TransactionWithDocument(DataSourceFactory dataSourceFactory)
    {
        _dataSourceFactory = dataSourceFactory;
    }

    public global::DataView.DataView GetDataView()
    {
        if (_dataView != null)
            return _dataView;

        var presentation = new DataViewPresentation(50, "fa-solid fa-money-bill-transfer", "Tranzakcje");

        _dataView = new ("t", "Tranzakcje", _dataSourceFactory.TransactionWithDocument, presentation)
        {
            SortingColumnPropertyName = "Date",
            SortingDescending = true,
            Columns = new (new List<DataViewColumn>
            {
                new DataViewColumnDate("Date", "Data", "d"),
                new DataViewColumnText("Account", "Konto", "a"),
                new DataViewColumnText("Category", "Kategoria", "ct"),
                //new PropertyInfoMoney("Amount", "Currency", "Kwota", "am"),
                new DataViewColumnText("Description", "Opis", "d"/*, TextFilterComponents.Default*/),
                new DataViewColumnText("Document.Company", "Firma dokumentu", "n")
            })
        };

        return _dataView;
    }
}