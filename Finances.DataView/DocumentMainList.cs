using DataViews;
using Finances.DataSource;

namespace FinancesDataView;

public class DocumentMainList : IDataView
{
    private readonly DataSourceFactory _dataSourceFactory;
    private DataView? _dataView;

    public DocumentMainList(DataSourceFactory dataSourceFactory)
    {
        _dataSourceFactory = dataSourceFactory;
    }

    public DataView GetDataView()
    {
        if (_dataView != null)
            return _dataView;

        var presentation = new DataViewPresentation(60, "fa-regular fa-folder-open", "Dokumenty");
        var columns = new List<DataViewColumn>
        {
            new DataViewColumnDocumentLink("Number", "Document", "l"),
            new DataViewColumnDate("Date", "Data", "d", DataViewColumnDateFilterComponents.Default),
            new DataViewColumnText("Description", "Opis", "de", DataViewColumnTextFilterComponents.Default),
        };

        _dataView = new("d", "Dokumenty", _dataSourceFactory.Document, new(columns), presentation);
        _dataView.Query.PreSorters.Add(columns.First(c => c.PrimaryDataColumnName == "Number"), true);
        return _dataView;
    }
}