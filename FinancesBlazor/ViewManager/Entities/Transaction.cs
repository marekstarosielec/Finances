using Finances.DataAccess;
using FinancesBlazor.DataAccess;
using FinancesBlazor.PropertyInfo;
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

        var presentation = new ViewPresentation(50, "fa-solid fa-money-bill-transfer", "Tranzakcje");

        _view = new View("t", "Tranzakcje", new BaseListService(new JsonListFile(configuration, "transactions.json")), presentation)
        {
            SortingColumnPropertyName = "Date",
            SortingDescending = true,
            Properties = new ReadOnlyCollection<PropertyInfoBase>(new List<PropertyInfoBase>
            {
                new PropertyInfoDate("Date", "Data", "d"),
                new PropertyInfoText("Account", "Konto", "a"),
                new PropertyInfoText("Category", "Kategoria", "ct"),
                new PropertyInfoPrecision("Amount", "Kwota", "am"),
                new PropertyInfoText("Description", "Opis", "d")
            })
        };

        return _view;
    }
}
