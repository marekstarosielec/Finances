using FinancesBlazor.Extensions;
using System.Text.Json.Nodes;

namespace FinancesBlazor.PropertyInfo;

public static class TextValueFormatter
{
    public static string? GetFormattedString(JsonNode? value, PropertyInfoBase property)
    {
        if (property == null)
            return string.Empty;
        if (property.PropertyName == null)
            return property.NullValue;
        
        var valueNode = value?.GetDeepNode(property);
        if (valueNode == null)
            return property.NullValue;

        return property.DataType switch
        {
            DataType.Text => GetFormattedText(valueNode, property),
            DataType.Date => GetFormattedDate(valueNode, property),
            DataType.Precision => GetFormattedPrecision(valueNode, property),
            DataType.Money => GetFormattedMoney(valueNode, property),
            _ => string.Empty,
        };
    }

    private static string? GetFormattedText(JsonNode? value, PropertyInfoBase property) => value?[property.DeepPropertyName]?.GetValue<string>() ?? property.NullValue;

    private static string? GetFormattedDate(JsonNode? value, PropertyInfoBase property)
    {
        if (value == null)
            return property.NullValue;
        _ = DateTime.TryParse(value?[property.DeepPropertyName]?.GetValue<string>(), out var castedValue);
        return castedValue.ToString("yyyy-MM-dd");
    }

    private static string? GetFormattedPrecision(JsonNode? value, PropertyInfoBase property)
    {
        if (value == null)
            return property.NullValue;
        return value?[property.DeepPropertyName]?.GetValue<decimal>().ToString(property.Format);
    }

    private static string? GetFormattedMoney(JsonNode? value, PropertyInfoBase property)
    {
        if (value == null)
            return property.NullValue;
        if (property.SecondPropertyName == null )
            return value?[property.DeepPropertyName]?.GetValue<decimal>().ToString(property.Format);
        return $"{value?[property.DeepPropertyName]?.GetValue<decimal>().ToString(property.Format)} {value?[property.DeepSecondPropertyName!]?.GetValue<string>()}";
    }
}
