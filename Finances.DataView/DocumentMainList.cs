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

        var presentation = new DataViewPresentation { NavMenuIndex = 60, NavMenuIcon = "fa-regular fa-folder-open", NavMenuTitle = "Dokumenty" };
        var columns = new List<DataViewColumn>
        {
            new DataViewColumnText("Id", "Id", "id", visible: false),
            new DataViewColumnText("Number", "Dokument", "l"),
            new DataViewColumnDate("Date", "Data", "d", DataViewColumnDateFilterComponents.Default),
            new DataViewColumnText("Description", "Opis", "de", DataViewColumnTextFilterComponents.Default),
            new DataViewColumnDocumentLink("FileLink", "Plik", "f")
        };

        _dataView = new("d", "Dokumenty", _dataSourceFactory.Document, new(columns), presentation, "dd");
        _dataView.Query.PreSorters.Add(columns.First(c => c.PrimaryDataColumnName == "Number"), true);
        return _dataView;
    }
}