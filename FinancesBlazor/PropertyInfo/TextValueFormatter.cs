using System.Text.Json.Nodes;

namespace FinancesBlazor.PropertyInfo;

public static class TextValueFormatter
{
    public static string? GetFormattedString(JsonNode? value, PropertyInfoBase property)
    {
        if (property == null)
            return string.Empty;
        if (property.PropertyName == null || value?[property.PropertyName] ==null)
            return property.NullValue;

        return property.DataType switch
        {
            DataType.Text => GetFormattedText(value, property),
            DataType.Date => GetFormattedDate(value, property),
            DataType.Precision => GetFormattedPrecision(value, property),
            DataType.Money => GetFormattedMoney(value, property),
            _ => string.Empty,
        };
    }

    private static string? GetFormattedText(JsonNode? value, PropertyInfoBase property) => value?[property.PropertyName]?.GetValue<string>() ?? property.NullValue;

    private static string? GetFormattedDate(JsonNode? value, PropertyInfoBase property)
    {
        if (value == null)
            return property.NullValue;
        _ = DateTime.TryParse(value?[property.PropertyName]?.GetValue<string>(), out var castedValue);
        return castedValue.ToString("yyyy-MM-dd");
    }

    private static string? GetFormattedPrecision(JsonNode? value, PropertyInfoBase property)
    {
        if (value == null)
            return property.NullValue;
        return value?[property.PropertyName]?.GetValue<decimal>().ToString(property.Format);
    }

    private static string? GetFormattedMoney(JsonNode? value, PropertyInfoBase property)
    {
        if (value == null)
            return property.NullValue;
        if (property.SecondPropertyName == null )
            return value?[property.PropertyName]?.GetValue<decimal>().ToString(property.Format);
        return $"{value?[property.PropertyName]?.GetValue<decimal>().ToString(property.Format)} {value?[property.SecondPropertyName]?.GetValue<string>()}";
    }
}
