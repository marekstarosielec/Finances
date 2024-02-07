namespace DataSource;

public class DataValue
{
    public object? OriginalValue { get; set; }
    public object? CurrentValue { get; set; }

    public DataValue(object? originalValue, object? currentValue)
    {
        OriginalValue = originalValue;
        CurrentValue = currentValue;
    }
}
