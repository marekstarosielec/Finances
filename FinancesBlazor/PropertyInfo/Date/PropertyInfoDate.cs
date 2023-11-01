namespace FinancesBlazor.PropertyInfo;

public class PropertyInfoDate : PropertyInfoBase
{
    public PropertyInfoDate(string propertyName, string title, string shortName, DateFilterComponents? filterComponent = null) : base(propertyName, DataType.Date, title, shortName, new FilterComponentFactory().GetDateFilterComponent(filterComponent))
    {
    }
}
