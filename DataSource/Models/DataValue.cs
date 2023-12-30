namespace DataSource;

public class DataValue
{
    public object? OriginalValue { get; }
    public object? CurrentValue { get; set; }

    public DataValue(object? value)
    {
        OriginalValue = value;
        CurrentValue = value;
    }
}
