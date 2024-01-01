using DataSource;

namespace DataViews;

public class DataViewColumnFilter
{
    public List<string>? StringValue { get; set; }

    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }

    public string? SecondaryStringValue { get; set; }

    public Equality Equality { get; set; }

    public DataColumnFilter GetPrimaryDataColumnFilter() 
        => new DataColumnFilter { 
            StringValue = StringValue?.Select(s => s.ToLowerInvariant()).ToList() ?? new List<string>(), 
            DateFrom = DateFrom, 
            DateTo = DateTo,
            Equality = Equality 
        };

    public DataColumnFilter? GetSecondaryDataColumnFilter()
        => SecondaryStringValue == null 
        ? null
        : new DataColumnFilter
        {
            StringValue = new List<string> { SecondaryStringValue }
        };


}
