using Finances.DataSource;
using System.Collections.ObjectModel;
using View;

namespace Finances.View;

public class TransactionWithDocument : IView
{
    private readonly DataSourceFactory _dataSourceFactory;
    private global::View.View? _view;

    public TransactionWithDocument(DataSourceFactory dataSourceFactory)
    {
        _dataSourceFactory = dataSourceFactory;
    }

    public global::View.View GetView()
    {
        if (_view != null)
            return _view;

        var presentation = new ViewPresentation(50, "fa-solid fa-money-bill-transfer", "Tranzakcje");

        _view = new ("t", "Tranzakcje", _dataSourceFactory.TransactionWithDocument, presentation)
        {
            SortingColumnPropertyName = "Date",
            SortingDescending = true,
            Columns = new (new List<ViewColumn>
            {
                new ViewColumnDate("Date", "Data", "d"),
                new ViewColumnText("Account", "Konto", "a"),
                new ViewColumnText("Category", "Kategoria", "ct"),
                //new PropertyInfoMoney("Amount", "Currency", "Kwota", "am"),
                new ViewColumnText("Description", "Opis", "d"/*, TextFilterComponents.Default*/),
                new ViewColumnText("Document.Company", "Firma dokumentu", "n")
            })
        };

        return _view;
    }
}