using Finances.DataAccess;
using FinancesBlazor.Components.Grid;
using FinancesBlazor.DataAccess;
using System.Collections.ObjectModel;

namespace FinancesBlazor.ViewManager;

public partial class ViewsList
{
    private View? _gas;

    public View Gas
    {
        get
        {
            if (_configuration == null)
                throw new InvalidOperationException();

            if (_gas != null)
                return _gas;

            var viewListParameters = new ViewListParameters
            {
                SortingColumnDataName = "Date",
                SortingDescending = true,
                Columns = new ReadOnlyCollection<Column>(new List<Column> {
                        new Column("d", "Data", "Date", DataTypes.Date),
                        new Column("m", "Licznik", "Meter", DataTypes.Precision, format: "# ##0.0", align: Align.Right),
                        new Column("g", "Komentarz", "Comment", DataTypes.Text) })
            };

            _gas = new View("g", "Gaz", new BaseListService(new JsonListFile(_configuration, "gas.json"), viewListParameters))
            {
                Parameters = viewListParameters
            };

            return _gas;
        }
    }
}
