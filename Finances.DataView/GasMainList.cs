using DataViews;
using Finances.DataSource;

namespace FinancesDataView;

public class GasMainList : IDataView
{
    private readonly DataSourceFactory _dataSourceFactory;
    private DataView? _dataView;

    public GasMainList(DataSourceFactory dataSourceFactory)
    {
        _dataSourceFactory = dataSourceFactory;
    }

    public DataView GetDataView()
    {
        if (_dataView != null)
            return _dataView;

        var presentation = new DataViewPresentation(100, "fa-solid fa-fire", "Gaz");
        var columns = new List<DataViewColumn>
        {
            new DataViewColumnDate("Date", "Data", "d", DataViewColumnDateFilterComponents.Default),
            new DataViewColumnPrecision("Meter", "Licznik", "m") { Format = "0.000" },
            new DataViewColumnAmount("Amount", "Currency", "Kwota", "am"),
            new DataViewColumnText("Comment", "Opis", "de", DataViewColumnTextFilterComponents.Default)
        };

        _dataView = new("g", "Gaz", _dataSourceFactory.GasAndTransactionWithDocument, new(columns), presentation);

        _dataView.Query.PreSorters.Add(columns.Single(c => c.ShortName == "d"), true);
        return _dataView;
    }
}