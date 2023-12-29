using DataSource;
using System.Collections.ObjectModel;
using System.Web;

namespace DataView;

public class DataView
{
    public IDataSource DataSource { get; }

    private DataQuery _query = new DataQuery();

    public string Name { get; }

    public string Title { get; set; }

    public ReadOnlyCollection<DataViewColumn> Columns { get; }

    public DataViewPresentation? Presentation { get; }

    public DataViewQuery Query { get; init; }

    public bool IsLoading { get; private set; } = true;
    public bool IsSaving { get; private set; } = false;

    public DataQueryResult? Result { get; private set; }

    private string? _detailsViewName { get; }


    public string Serialize()
    {
        var data = Query.Serialize();
        return new StreamReader(new FormUrlEncodedContent(data).ReadAsStream()).ReadToEnd();
    }

    public void Deserialize(string serializedValue)
    {
        if (serializedValue == null)
            return;

        var items = HttpUtility.ParseQueryString(serializedValue);
        Query.Deserialize(items);
    }

    public void RemoveCache()
    {
        DataSource.RemoveCache();
        IsLoading = true;
    }

    public async Task Requery()
    {
        IsLoading = true;
        Query.Apply();
        Result = await DataSource.ExecuteQuery(_query);
        IsLoading = false;
    }

    public DataView(string name, string title, IDataSource dataSource, ReadOnlyCollection<DataViewColumn> columns, DataViewPresentation? presentation = null, string? detailsViewName = null)
    {
        Name = name;
        Title = title;
        DataSource = dataSource;
        Presentation = presentation;
        Columns = columns;
        Query = new DataViewQuery(_query, DataSource, Columns);
        ValidateColumns();
        _detailsViewName = detailsViewName;
    }

    void ValidateColumns()
    {
        foreach (DataViewColumn column in Columns)
        {
            if (!DataSource.Columns.ContainsKey(column.PrimaryDataColumnName))
                throw new InvalidOperationException($"DataViewColumn {column.ShortName} refers to DataColumn {column.PrimaryDataColumnName} which does not exist.");
            if (column.SecondaryDataColumnName != null && !DataSource.Columns.ContainsKey(column.SecondaryDataColumnName))
                throw new InvalidOperationException($"DataViewColumn {column.ShortName} refers to DataColumn {column.SecondaryDataColumnName} which does not exist.");
        }
    }

    public string? GetDetailsDataViewName()
    {
        return _detailsViewName;
    }

    public async Task Save(DataRow row)
    {
        IsSaving = true;
        await DataSource.Save(row);
        IsSaving = false;
    }
}
