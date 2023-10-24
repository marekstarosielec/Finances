﻿using Finances.DataAccess;
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
                Columns = new ReadOnlyCollection<Column>(new List<Column> {
                        new Column("d", "Data", "Date", DataTypes.Date),
                        new Column("m", "Licznik", "Meter", DataTypes.Precision, format: "# ##0.0", align: Align.Right),
                        new Column("c", "Komentarz", "Comment", DataTypes.Text) })
            };

            _electricity = new View("e", "Prąd", new BaseListService(new JsonListFile(_configuration, "electricity.json"), viewListParameters))
            {
                Parameters = viewListParameters
            };

            return _electricity;
        }
    }
}