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

        var presentation = new ViewPresentation(110, "fa-solid fa-fire-flame-simple", "Gaz");

        _view = new View("g", "Gaz", new BaseListService(new JsonListFile(configuration, "gas.json")), presentation)
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
