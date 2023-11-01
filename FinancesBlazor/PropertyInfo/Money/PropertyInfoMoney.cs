namespace FinancesBlazor.PropertyInfo;

public class PropertyInfoMoney : PropertyInfoBase
{
    public PropertyInfoMoney(string valuePropertyName, string currencypropertyName, string title, string shortName) : base(valuePropertyName, DataType.Money, title, shortName, null)
    {
        SecondPropertyName = currencypropertyName;
        Format = "# ##0.0";
        HorizontalAlign = ContentAlign.Right;
    }
}
