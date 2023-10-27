using Finances.DataAccess;
using FinancesBlazor.DataAccess;
using FinancesBlazor.DataTypes;
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

        var viewListParameters = new ViewListParameters
        {
            SortingColumnDataName = "Date",
            SortingDescending = true,
            Columns = new ReadOnlyCollection<Column>(new List<Column> {
                new Column("gd", "Data Gaz", "Date", DataTypesList.Date),
                new Column("gm", "Licznik", "Meter", DataTypesList.Precision, format: "# ##0.0", align: Align.Right),
                new Column("gc", "Komentarz", "Comment", DataTypesList.Text) })
        };

        var presentation = new ViewPresentation(110, "fa-solid fa-fire-flame-simple", "Gaz");

        _view = new View("g", "Gaz", new BaseListService(new JsonListFile(configuration, "gas.json"), viewListParameters), presentation)
        {
            Parameters = viewListParameters
        };

        return _view;
    }
}
