namespace FinancesBlazor.PropertyInfo;

public abstract class PropertyInfoBase
{
    public string PropertyName { get; }

    public string DeepPropertyName => PropertyName.Split('.').Last();

    public string? SecondPropertyName { get; set; }

    public string? DeepSecondPropertyName => SecondPropertyName?.Split('.').Last();

    public DataType DataType { get; }

    public string Title { get; }
    
    public string ShortName { get; set; }

    public string? NullValue { get; set; }

    public string? Format { get; set; }

    public ContentAlign? HorizontalAlign { get; set; }

    public Type? FilterComponentType { get; set; }

    public PropertyInfoBase(string propertyName, DataType dataType, string title, string shortName, Type? filterComponentType)
    {
        PropertyName = propertyName;
        DataType = dataType;
        Title = title;
        ShortName = shortName;
        FilterComponentType = filterComponentType;
    }
}
