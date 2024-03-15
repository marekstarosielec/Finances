using DataSource;
using DataViews;
using Finances.DataSource;

namespace FinancesDataView;

public class TransactionAccountMainList : IDataView
{
    private readonly DataSourceFactory _dataSourceFactory;
    private DataView? _dataView;

    public TransactionAccountMainList(DataSourceFactory dataSourceFactory)
    {
        _dataSourceFactory = dataSourceFactory;
    }

    public DataView GetDataView()
    {
        if (_dataView != null)
            return _dataView;

        var presentation = new DataViewPresentation(2060, "fa-solid fa-money-bill-transfer", "Konta");
        var columns = new List<DataViewColumn>
        {
            new DataViewColumnText("Id", "Id", "id", visible: false),
            new DataViewColumnText("Title", "Nazwa", "t"),
            new DataViewColumnText("Currency", "Waluta", "c"),
            new DataViewColumnCheckbox("Deleted", "Usunięte", "d"),
        };

        _dataView = new("ta", "Konta", _dataSourceFactory.TransactionAccount, new(columns), presentation, "tad");
        //_dataView.Query.Prefilters.Add(
        //    new Prefilter(
        //        name: "savings",
        //        title: "Ukryj usunięte",
        //        column: columns.Single(c => c.PrimaryDataColumnName == "Deleted"),
        //        columnFilter: new DataViewColumnFilter
        //        {
        //            StringValue = new List<bool> { true },
        //            Equality = DataSource.Equality.NotEquals
        //        },
        //        applied: true
        //    ));
        _dataView.Query.PreSorters.Add(columns.Single(c => c.PrimaryDataColumnName == "Title"), false);
        return _dataView;
    }
}