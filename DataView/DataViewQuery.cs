using DataSource;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Web;

namespace DataView;

public class DataViewQuery
{
    private readonly DataQuery _dataQuery;
    private readonly IDataSource _dataSource;
    private readonly ReadOnlyCollection<DataViewColumn> _columns;

    public List<string> IdFilters { get; } = new();
    public Dictionary<DataViewColumn, DataViewColumnFilter> Filters { get; } = new ();
    public Dictionary<DataViewColumn, bool> Sorters = new();
    public int PageSize = 100;

    public List<Prefilter> Prefilters { get; set; } = new();
    public Dictionary<DataViewColumn, bool> PreSorters = new();

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

    internal Dictionary<string, string> Serialize()
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
                if (filter.Value.StringValue?.Count > 0)
                {
                    data[$"f_{filter.Key.ShortName}_sv"] = string.Join(',', filter.Value.StringValue);
                    data[$"f_{filter.Key.ShortName}_eq"] = filter.Value.Equality.ToString();
                }
                if (filter.Value.DateFrom != null)
                    data[$"f_{filter.Key.ShortName}_fr"] = filter.Value.DateFrom.Value.ToString("yyyyMMdd");
                if (filter.Value.DateTo != null)
                    data[$"f_{filter.Key.ShortName}_to"] = filter.Value.DateTo.Value.ToString("yyyyMMdd");
            }
        if (PageSize != 100)
            data["ps"] = PageSize.ToString();
        return data;
    }

    internal void Deserialize(NameValueCollection items)
    {
        Sorters.Clear();
        Filters.Clear();
        foreach (string key in items)
        {
            if (key == null)
                continue;

            if (key.StartsWith("s_"))
            {
                var dataViewColumnShortName = key[2..];
                var dataViewColumn = _columns.FirstOrDefault(c => c.ShortName == dataViewColumnShortName);
                if (dataViewColumn == null)
                    continue; //In case someone edited column name directly in url.

                Sorters[dataViewColumn] = items[key] == "1" ? true : false;
            }
            else if (key?.StartsWith("f_") == true && key?.EndsWith("_sv") == true)
            {
                var dataViewColumnShortName = key[2..^3];
                var dataViewColumn = _columns.FirstOrDefault(c => c.ShortName == dataViewColumnShortName);
                if (dataViewColumn == null)
                    continue; //In case someone edited column name directly in url.

                Filters[dataViewColumn] = new DataViewColumnFilter { 
                    StringValue = items[key].Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                };
            }
            else if (key?.StartsWith("f_") == true && key?.EndsWith("_fr") == true)
            {
                if (items[key] == null)
                    continue;
                var dataViewColumnShortName = key[2..^3];
                var dataViewColumn = _columns.FirstOrDefault(c => c.ShortName == dataViewColumnShortName);
                if (dataViewColumn == null)
                    continue; //In case someone edited column name directly in url.

                if (!Filters.ContainsKey(dataViewColumn))
                    Filters.Add(dataViewColumn, new DataViewColumnFilter());
                Filters[dataViewColumn].DateFrom = DateTime.ParseExact(items[key], "yyyyMMdd", null);
            }
            else if (key?.StartsWith("f_") == true && key?.EndsWith("_to") == true)
            {
                if (items[key] == null)
                    continue;

                var dataViewColumnShortName = key[2..^3];
                var dataViewColumn = _columns.FirstOrDefault(c => c.ShortName == dataViewColumnShortName);
                if (dataViewColumn == null)
                    continue; //In case someone edited column name directly in url.

                if (!Filters.ContainsKey(dataViewColumn))
                    Filters.Add(dataViewColumn, new DataViewColumnFilter());
                Filters[dataViewColumn].DateTo = DateTime.ParseExact(items[key], "yyyyMMdd", null);
            }
            else if (key?.StartsWith("f_") == true && key?.EndsWith("_eq") == true)
            {
                var dataViewColumnShortName = key[2..^3];
                var dataViewColumn = _columns.FirstOrDefault(c => c.ShortName == dataViewColumnShortName);
                if (dataViewColumn == null)
                    continue; //In case someone edited column name directly in url.

                if (!Enum.TryParse<Equality>(items[key], out var equality))
                    continue; //In case someone edited column name directly in url.

                if (!Filters.ContainsKey(dataViewColumn))
                    Filters.Add(dataViewColumn, new DataViewColumnFilter());
                Filters[dataViewColumn].Equality = equality;
            }
            else if (key =="ps")
            {
                if (!int.TryParse(items[key], out var ps))
                    continue;

                PageSize = ps;
            }
        }

    }

    public void Reset()
    {
        Sorters.Clear();
        foreach (var preSorter in PreSorters)
            Sorters.Add(preSorter.Key, preSorter.Value);
        
        Filters.Clear();
        foreach (var preFilter in Prefilters)
            preFilter.Applied = preFilter.DefaultApplied;
    }
}
