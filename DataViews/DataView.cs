using DataSource;
using System.Collections.ObjectModel;
using System.Web;

namespace DataViews;

public class DataView
{
    public IDataSource DataSource { get; }

    private DataQuery _dataQuery = new DataQuery();

    public string Name { get; }

    public string Title { get; set; }

    public ReadOnlyCollection<DataViewColumn> Columns { get; }

    public DataViewPresentation? Presentation { get; set;  }

    public DataViewQuery Query { get; init; }

    public bool IsLoading { get; private set; } = true;
    public bool IsSaving { get; private set; } = false;

    public bool IsCacheInvalidated => DataSource.IsCacheInvalidated;

    public DataQueryResult? Result { get; private set; }

    private string? _detailsViewName { get; }

    public DataView(string name, string title, IDataSource dataSource, ReadOnlyCollection<DataViewColumn> columns, DataViewPresentation? presentation = null, string? detailsViewName = null)
    {
        Name = name;
        Title = title;
        DataSource = dataSource;
        Presentation = presentation;
        Columns = columns;
        Query = new DataViewQuery(_dataQuery, DataSource, Columns);
        ValidateColumns();
        _detailsViewName = detailsViewName;
    }

    //public DataView Clone(string name)
    //{
    //    var clonedColumns = new Collection<DataViewColumn>();
    //    foreach (DataViewColumn column in Columns)
    //        clonedColumns.Add(column.Clone());
    //    return new DataView(name, Title, DataSource, new ReadOnlyCollection<DataViewColumn>(clonedColumns), Presentation?.Clone(), _detailsViewName);
    //}

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
        Console.WriteLine($"{Name}: removing cache");
        DataSource.RemoveCache();
        IsLoading = true;
    }

    /// <summary>
    /// Reloads data from data source.
    /// </summary>
    /// <param name="preservePrevious">If same rows are in previous query, they are used instead of new values. 
    /// That allows to preserve changes in edited details in case view is reloaded (e.g. new detail view is added).</param>
    /// <returns></returns>
    public async Task Requery(bool preservePrevious = false)
    {
        IsLoading = true;
        Query.ApplyToDataQuery();
        var previousResultRows = preservePrevious ? Result?.Clone().Rows.ToList() : null;
        Result = await DataSource.ExecuteQuery(_dataQuery);
        var resultRows = Result.Rows.ToList();
        if (previousResultRows != null)
        {
            foreach (var previousResultRow in previousResultRows)
            {
                var i = resultRows.FindIndex(r => r.Id.OriginalValue == previousResultRow.Id.OriginalValue);
                if (i == -1)
                    continue;
                resultRows[i] = previousResultRow;
            }
            Result = new DataQueryResult(Result.Columns, resultRows, Result.TotalRowCount);
        }
        IsLoading = false;
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

    public async Task Save(List<DataRow> rows)
    {
        IsSaving = true;
        await DataSource.Save(rows);
        IsSaving = false;
    }
}

