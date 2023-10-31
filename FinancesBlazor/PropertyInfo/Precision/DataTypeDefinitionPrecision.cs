namespace FinancesBlazor.PropertyInfo;

public class PropertyInfoPrecision : PropertyInfoBase
{
    public PropertyInfoPrecision(string propertyName, string title, string shortName) : base(propertyName, DataType.Precision, title, shortName, null) 
    {
        Format = "# ##0.0";
        HorizontalAlign = ContentAlign.Right;
    }
}
