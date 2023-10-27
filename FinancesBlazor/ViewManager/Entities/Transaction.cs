using Finances.DataAccess;
using FinancesBlazor.DataAccess;
using FinancesBlazor.DataTypes;
using System.Collections.ObjectModel;

namespace FinancesBlazor.ViewManager;

public class Gas : IEntity
{
    private View? _view;

    public View GetView(IConfiguration configuration)
    {
        if (_view != null)
            return _view;

        if (configuration == null)
            throw new InvalidOperationException();

        var viewListParameters = new ViewListParameters
        {
            SortingColumnDataName = "Date",
            SortingDescending = true,
            Columns = new ReadOnlyCollection<Column>(new List<Column> {
                new Column("d", "Data", "Date", DataTypesList.Date),
                new Column("a", "Konto", "Account", DataTypesList.Text),
                new Column("ct", "Kategoria", "Category", DataTypesList.Text),
                new Column("am", "Kwota", "Amount", DataTypesList.Precision),
                new Column("d", "Opis", "Description", DataTypesList.Text)})
        };

        var presentation = new ViewPresentation(50, "fa-solid fa-money-bill-transfer", "Tranzakcje");

        _view = new View("t", "Tranzakcje", new BaseListService(new JsonListFile(configuration, "transactions.json"), viewListParameters), presentation)
        {
            Parameters = viewListParameters
        };

        return _view;
    }
}
