namespace DataSource;

public class DataValue
{
    
    public object? OriginalValue { get; set; }

    private object? currentValue;
    public object? CurrentValue
    {
        get => currentValue; 
        set
        {
            if (IsReadOnly)
                throw new InvalidOperationException("Cannot modify read only value");
            currentValue = value;
        }
    }

    public bool IsReadOnly { get; }

    public DataValue(object? originalValue)
    {
        OriginalValue = originalValue;
        CurrentValue = originalValue;
        IsReadOnly = true;
    }

    public DataValue(object? originalValue, object? currentValue)
    {
        OriginalValue = originalValue;
        CurrentValue = currentValue;
    }
}
