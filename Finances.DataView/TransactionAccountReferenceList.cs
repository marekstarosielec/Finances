using DataSource;
using DataViews;
using Finances.DataSource;

namespace FinancesDataView;

public class TransactionAccountReferenceList : IDataView
{
    private readonly DataSourceFactory _dataSourceFactory;
    private DataView? _dataView;

    public TransactionAccountReferenceList(DataSourceFactory dataSourceFactory)
    {
        _dataSourceFactory = dataSourceFactory;
    }

    public DataView GetDataView()
    {
        if (_dataView != null)
            return _dataView;

        var presentation = new DataViewPresentation { ShowSelectionCheckboxesInList = false, ShowToolbar = false, ShowHeaders = false };
        var columns = new List<DataViewColumn>
        {
            new DataViewColumnText("Id", "Id", "id", visible: false),
            new DataViewColumnText("Title", "Nazwa", "t"),
            new DataViewColumnText("Currency", "Waluta", "c", visible: false),
            new DataViewColumnCheckbox("Deleted", "Usunięte", "d", visible: false),
        };

        _dataView = new("tar", "Konta", _dataSourceFactory.TransactionAccount, new(columns), presentation, "tad");
        _dataView.Query.Prefilters.Add(
            new Prefilter(
                name: "deleted",
                title: "Ukryj usunięte",
                column: columns.Single(c => c.PrimaryDataColumnName == "Deleted"),
                columnFilter: new DataViewColumnFilter
                {
                    BoolValue = false,
                    Equality = Equality.Equals
                },
                applied: true
            ));
        _dataView.Query.PreSorters.Add(columns.Single(c => c.PrimaryDataColumnName == "Title"), false);
        return _dataView;
    }
}