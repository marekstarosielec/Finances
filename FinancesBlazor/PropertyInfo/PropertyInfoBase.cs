﻿namespace FinancesBlazor.PropertyInfo;

public abstract class PropertyInfoBase
{
    public string PropertyName { get; }

    public DataType DataType { get; }

    public string Title { get; }
    
    public string ShortName { get; set; }

    public string? NullValue { get; set; }

    public string? Format { get; set; }

    public ContentAlign? HorizontalAlign { get; set; }

    public PropertyInfoBase(string propertyName, DataType dataType, string title, string shortName)
    {
        PropertyName = propertyName;
        DataType = dataType;
        Title = title;
        ShortName = shortName;
    }
}
