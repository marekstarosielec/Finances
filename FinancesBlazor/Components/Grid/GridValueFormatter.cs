using FinancesBlazor.PropertyInfo;
using System.Text.Json.Nodes;

namespace FinancesBlazor.Components.Grid;

public static class GridValueFormatter
{
    public static string? GetFormattedString(JsonNode? value, PropertyInfoBase property) => property.DataType switch
    {
        DataType.Text => GetFormattedText(value, property),
        DataType.Date => GetFormattedDate(value, property),
        DataType.Precision => GetFormattedPrecision(value, property),
        _ => string.Empty,
    };

    private static string? GetFormattedText(JsonNode? value, PropertyInfoBase property) => value?.GetValue<string>() ?? property.NullValue;

    private static string? GetFormattedDate(JsonNode? value, PropertyInfoBase property)
    {
        if (value == null)
            return property.NullValue;
        _ = DateTime.TryParse(value.GetValue<string>(), out var castedValue);
        return castedValue.ToString("yyyy-MM-dd");
    }

    private static string? GetFormattedPrecision(JsonNode? value, PropertyInfoBase property)
    {
        if (value == null)
            return property.NullValue;
        return value.GetValue<decimal>().ToString(property.Format);
    }
}
