using Finances.Entities.Electricity;
using FinancesBlazor.Components.Grid;

namespace FinancesBlazor.Entities;

public partial class ViewsList
{
    private View? _field;

    public View Electricity
    {
        get
        {
            if (_field != null)
                return _field;

            _field = new View("electricity", "Prąd", new ElectricityService2(_configuration))
            {
                MainGridSettings = new GridSettings("e")
                {
                    SortingColumnDataName = "Date",
                    SortingDescending = true,
                    Columns = new List<GridColumn> {
                new GridColumn("Data", "Date", DataAccess.DataTypes.Date),
                new GridColumn("Licznik", "Meter", DataAccess.DataTypes.Precision, format: "0.0"),
                new GridColumn("Komentarz", "Comment", DataAccess.DataTypes.Text) }
                .ToArray()
                }
            };
            return _field;
        }
    }
}
