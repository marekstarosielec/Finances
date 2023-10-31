namespace FinancesBlazor.PropertyInfo;

public class PropertyInfoText : PropertyInfoBase
{
    public PropertyInfoText(string propertyName, string title, string shortName, TextFilterComponents? filterComponent = null) : base(propertyName, DataType.Text, title, shortName, new FilterComponentFactory().GetTextFilterComponent(filterComponent))
    {
    }
}
