namespace DataSource;

public class DataColumnFilter
{
    public List<string> StringValue { get; set; }

    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }

    public Equality Equality { get; set; }
}
