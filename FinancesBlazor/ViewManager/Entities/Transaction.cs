using Finances.DataAccess;
using FinancesBlazor.DataAccess;
using FinancesBlazor.PropertyInfo;
using System.Collections.ObjectModel;

namespace FinancesBlazor.ViewManager;

public class Transaction : IEntity
{
    private View? _view;

    public View GetView(IConfiguration configuration)
    {
        if (_view != null)
            return _view;

        if (configuration == null)
            throw new InvalidOperationException();

        var presentation = new ViewPresentation(50, "fa-solid fa-money-bill-transfer", "Tranzakcje");
        var jsonListFile = new JsonListFile(configuration, "transactions.json", new JoinDefinition("documents.json", "DocumentId", "Document"));

        _view = new View("t", "Tranzakcje", new BaseListService(jsonListFile), presentation)
        {
            SortingColumnPropertyName = "Date",
            SortingDescending = true,
            Properties = new ReadOnlyCollection<PropertyInfoBase>(new List<PropertyInfoBase>
            {
                new PropertyInfoDate("Date", "Data", "d", DateFilterComponents.Default),
                new PropertyInfoText("Account", "Konto", "a"),
                new PropertyInfoText("Category", "Kategoria", "ct"),
                new PropertyInfoMoney("Amount", "Currency", "Kwota", "am"),
                new PropertyInfoText("Description", "Opis", "d", TextFilterComponents.Default),
                new PropertyInfoText("Document.Company", "Firma dokumentu", "n")
            })
        };

        return _view;
    }
}
