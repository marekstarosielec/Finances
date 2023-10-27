using Finances.DataAccess;
using FinancesBlazor.DataAccess;
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

        var viewListParameters = new ViewListParameters
        {
            SortingColumnDataName = "Date",
            SortingDescending = true,
            Columns = new ReadOnlyCollection<Column>(new List<Column> {
                        new Column("d", "Data", "Date", DataTypes.Date),
                        new Column("m", "Licznik", "Meter", DataTypes.Precision, format: "# ##0.0", align: Align.Right),
                        new Column("c", "Komentarz", "Comment", DataTypes.Text) })
        };

        var presentation = new ViewPresentation(100, "fa-solid fa-bolt", "Prąd");

        _view = new View("e", "Prąd", new BaseListService(new JsonListFile(configuration, "electricity.json"), viewListParameters), presentation)
        {
            Parameters = viewListParameters
        };

        return _view;
    }
}
