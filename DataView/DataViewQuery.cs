using DataSource;
using System.Collections.ObjectModel;
using System.Web;

namespace DataView;

public class DataViewQuery
{
    private readonly DataQuery _dataQuery;
    private readonly IDataSource _dataSource;
    private readonly ReadOnlyCollection<DataViewColumn> _columns;

    public Dictionary<DataViewColumn, DataViewColumnFilter> Filters { get; } = new ();
    public Dictionary<DataViewColumn, bool> Sorters = new();
    public int PageSize = 100;

    public List<Prefilter> Prefilters { get; set; } = new();

    public DataViewQuery(DataQuery dataQuery, IDataSource dataSource, ReadOnlyCollection<DataViewColumn> columns)
    {
        _dataQuery = dataQuery;
        _dataSource = dataSource;
        _columns = columns;
    }

    internal void Apply()
    {
        _dataQuery.Sorters.Clear();
        foreach (var sorter in Sorters)
        {
            var dataColumn = _dataSource.Columns.FirstOrDefault(c => c.Key == sorter.Key.PrimaryDataColumnName).Value;
            if (dataColumn == null)
                throw new InvalidOperationException($"Cannot find column named {sorter.Key.PrimaryDataColumnName}");
            _dataQuery.Sorters[dataColumn] = sorter.Value;
        }

        _dataQuery.Filters.Clear();
        foreach (var filter in Filters)
        {
            var dataColumn = _dataSource.Columns.FirstOrDefault(c => c.Key == filter.Key.PrimaryDataColumnName).Value;
            if (dataColumn == null)
                throw new InvalidOperationException($"Cannot find column named {filter.Key.PrimaryDataColumnName}");
            _dataQuery.Filters[dataColumn] = filter.Value.GetPrimaryDataColumnFilter();

            if (filter.Key.SecondaryDataColumnName == null)
                continue;

            dataColumn = _dataSource.Columns.FirstOrDefault(c => c.Key == filter.Key.SecondaryDataColumnName).Value;
            if (dataColumn == null)
                throw new InvalidOperationException($"Cannot find column named {filter.Key.SecondaryDataColumnName}");

            var filterValue = filter.Value.GetSecondaryDataColumnFilter();
            if (filterValue == null)
                continue;

            _dataQuery.Filters[dataColumn] = filterValue;
        }

        foreach (var prefilter in Prefilters)
        {
            if (!prefilter.Applied)
                continue;

            var dataColumn = _dataSource.Columns.FirstOrDefault(c => c.Key == prefilter.Column.PrimaryDataColumnName).Value;
            if (dataColumn == null)
                throw new InvalidOperationException($"Cannot find column named {prefilter.Column.PrimaryDataColumnName}");
            _dataQuery.Filters[dataColumn] = prefilter.ColumnFilter.GetPrimaryDataColumnFilter();
        }

        _dataQuery.PageSize = PageSize;
    }

    public string Serialize()
    {
        var data = new Dictionary<string, string>();
        if (Sorters != null)
            foreach (var sorter in Sorters)
            {
                data[$"s_{sorter.Key.ShortName}"] = sorter.Value ? "1" : "0";
            }

        if (Filters != null)
            foreach (var filter in Filters)
            {
                if (!string.IsNullOrWhiteSpace(filter.Value.StringValue))
                    data[$"f_{filter.Key.ShortName}_sv"] = filter.Value.StringValue;
                if (filter.Value.DateFrom != null)
                    data[$"f_{filter.Key.ShortName}_fr"] = filter.Value.DateFrom.Value.ToString("yyyyMMdd");
                if (filter.Value.DateTo != null)
                    data[$"f_{filter.Key.ShortName}_to"] = filter.Value.DateTo.Value.ToString("yyyyMMdd");
                data[$"f_{filter.Key.ShortName}_eq"] = filter.Value.Equality.ToString();
            }
        if (PageSize != 100)
            data["ps"] = PageSize.ToString();
        return new StreamReader(new FormUrlEncodedContent(data).ReadAsStream()).ReadToEnd();
    }

    public void Deserialize(string serializedValue)
    {
        Sorters.Clear();
        Filters.Clear();
        if (serializedValue == null)
            return;

        var nvc = HttpUtility.ParseQueryString(serializedValue);
        var items = nvc.AllKeys.SelectMany(nvc.GetValues, (k, v) => new { key = k, value = v });
        foreach (var item in items)
        {
            if (item.key == null)
                continue;

            if (item.key.StartsWith("s_"))
            {
                var dataViewColumnShortName = item.key[2..];
                var dataViewColumn = _columns.FirstOrDefault(c => c.ShortName == dataViewColumnShortName);
                if (dataViewColumn == null)
                    continue; //In case someone edited column name directly in url.

                Sorters[dataViewColumn] = item.value == "1" ? true : false;
            }
            else if (item.key?.StartsWith("f_") == true && item.key?.EndsWith("_sv") == true)
            {
                var dataViewColumnShortName = item.key[2..^3];
                var dataViewColumn = _columns.FirstOrDefault(c => c.ShortName == dataViewColumnShortName);
                if (dataViewColumn == null)
                    continue; //In case someone edited column name directly in url.

                Filters[dataViewColumn] = new DataViewColumnFilter { 
                    StringValue = item.value 
                };
            }
            else if (item.key?.StartsWith("f_") == true && item.key?.EndsWith("_fr") == true)
            {
                var dataViewColumnShortName = item.key[2..^3];
                var dataViewColumn = _columns.FirstOrDefault(c => c.ShortName == dataViewColumnShortName);
                if (dataViewColumn == null)
                    continue; //In case someone edited column name directly in url.

                Filters[dataViewColumn] ??= new DataViewColumnFilter();
                Filters[dataViewColumn].DateFrom = DateTime.ParseExact(item.value, "yyyyMMdd", null);
            }
            else if (item.key?.StartsWith("f_") == true && item.key?.EndsWith("_to") == true)
            {
                var dataViewColumnShortName = item.key[2..^3];
                var dataViewColumn = _columns.FirstOrDefault(c => c.ShortName == dataViewColumnShortName);
                if (dataViewColumn == null)
                    continue; //In case someone edited column name directly in url.

                Filters[dataViewColumn] ??= new DataViewColumnFilter();
                Filters[dataViewColumn].DateTo = DateTime.ParseExact(item.value, "yyyyMMdd", null);
            }
            else if (item.key?.StartsWith("f_") == true && item.key?.EndsWith("_eq") == true)
            {
                var dataViewColumnShortName = item.key[2..^3];
                var dataViewColumn = _columns.FirstOrDefault(c => c.ShortName == dataViewColumnShortName);
                if (dataViewColumn == null)
                    continue; //In case someone edited column name directly in url.

                if (!Enum.TryParse<Equality>(item.value, out var equality))
                    continue; //In case someone edited column name directly in url.

                Filters[dataViewColumn] ??= new DataViewColumnFilter();
                Filters[dataViewColumn].Equality = equality;
            }
            else if (item.key=="ps")
            {
                if (!int.TryParse(item.value, out var ps))
                    continue;

                PageSize = ps;
            }
        }

    }
}
