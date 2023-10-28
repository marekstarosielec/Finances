using Finances.DataAccess;
using FinancesBlazor.DataAccess;
using FinancesBlazor.PropertyInfo;
using System.Collections.ObjectModel;

namespace FinancesBlazor.ViewManager;

public class Electricity : IEntity
{
    private View? _view;

    public View GetView(IConfiguration configuration)
    {
        if (_view != null)
            return _view;
        
        if (configuration == null)
            throw new InvalidOperationException();

        var presentation = new ViewPresentation(100, "fa-solid fa-bolt", "Prąd");

        _view = new View("e", "Prąd", new BaseListService(new JsonListFile(configuration, "electricity.json")), presentation)
        {
            SortingColumnPropertyName = "Date",
            SortingDescending = true,
            Properties = new ReadOnlyCollection<PropertyInfoBase>(new List<PropertyInfoBase>
            {
                new PropertyInfoDate("Date", "Data", "d"),
                new PropertyInfoPrecision("Meter", "Licznik", "m"),
                new PropertyInfoText("Comment", "Komentarz", "c")
            })
        };

        return _view;
    }
}
