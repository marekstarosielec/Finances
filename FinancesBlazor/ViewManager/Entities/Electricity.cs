using Finances.DataAccess;
using FinancesBlazor.Components.Grid;
using FinancesBlazor.DataAccess;
using System.Collections.ObjectModel;

namespace FinancesBlazor.ViewManager;

public partial class ViewsList
{
    private View? _electricity;

    public View Electricity
    {
        get
        {
            if (_configuration == null)
                throw new InvalidOperationException();

            if (_electricity != null)
                return _electricity;

            var viewListParameters = new ViewListParameters
            {
                SortingColumnDataName = "Date",
                SortingDescending = true,
                Columns = new ReadOnlyCollection<GridColumn>(new List<GridColumn> {
                        new GridColumn("Data", "Date", DataTypes.Date),
                        new GridColumn("Licznik", "Meter", DataTypes.Precision, format: "# ##0.0"),
                        new GridColumn("Komentarz", "Comment", DataTypes.Text) })
            };

            _electricity = new View("electricity", "Prąd", new BaseListService(new JsonListFile(_configuration, "electricity.json"), viewListParameters))
            {
                Parameters = viewListParameters
            };

            return _electricity;
        }
    }
}
