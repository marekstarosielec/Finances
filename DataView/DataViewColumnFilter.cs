using DataSource;

namespace DataView;

public class DataViewColumnFilter
{
    public string? StringValue { get; set; }

    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }

    public string? SecondaryStringValue { get; set; }

    public DataColumnFilter GetPrimaryDataColumnFilter() 
        => new DataColumnFilter { 
            StringValue = StringValue, 
            DateFrom = DateFrom, 
            DateTo = DateTo 
        };

    public DataColumnFilter? GetSecondaryDataColumnFilter()
        => SecondaryStringValue == null 
        ? null
        : new DataColumnFilter
        {
            StringValue = SecondaryStringValue
        };
}
