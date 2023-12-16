using DataView;
using Finances.DataSource;

namespace FinancesDataView;

public class TransactionMainList : IDataView
{
    private readonly DataSourceFactory _dataSourceFactory;
    private DataView.DataView? _dataView;

    public TransactionMainList(DataSourceFactory dataSourceFactory)
    {
        _dataSourceFactory = dataSourceFactory;
    }

    public DataView.DataView GetDataView()
    {
        if (_dataView != null)
            return _dataView;

        var presentation = new DataViewPresentation(50, "fa-solid fa-money-bill-transfer", "Tranzakcje");
        var columns = new List<DataViewColumn>
        {
            new DataViewColumnDate("Date", "Data", "d", DataViewColumnDateFilterComponents.Default),
            new DataViewColumnText("Account", "Konto", "a"),
            new DataViewColumnText("Category", "Kategoria", "ct"),
            new DataViewColumnAmount("Amount", "Currency", "Kwota", "am"),
            new DataViewColumnText("Description", "Opis", "de", DataViewColumnTextFilterComponents.Default),
            new DataViewColumnText("DocumentNumber", "Numer dokumentu", "n")
        };

        _dataView = new("t", "Tranzakcje", _dataSourceFactory.TransactionWithDocument, new(columns), presentation, "td");
        _dataView.Query.Prefilters.Add(
            new Prefilter(
                name: "savings", 
                title: "Ukryj Oszczędzanie", 
                column: columns.Single(c => c.PrimaryDataColumnName == "Category"),
                columnFilter: new DataViewColumnFilter { 
                    StringValue = new List<string> { "Oszczędzanie" },
                    Equality = DataSource.Equality.NotEquals
                },
                applied: true
            ));
        _dataView.Query.PreSorters.Add(columns.Single(c => c.PrimaryDataColumnName == "Date"), true);
        return _dataView;
    }
}