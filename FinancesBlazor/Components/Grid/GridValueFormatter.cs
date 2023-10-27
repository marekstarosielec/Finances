using FinancesBlazor.DataTypes;
using FinancesBlazor.ViewManager;
using System.Text.Json.Nodes;

namespace FinancesBlazor.Components.Grid;

public static class GridValueFormatter
{
    public static string GetFormattedString(JsonNode? value, Column column) => column.DataType switch
    {
        DataTypesList.Text => GetFormattedText(value, column),
        DataTypesList.Date => GetFormattedDate(value, column),
        DataTypesList.Precision => GetFormattedPrecision(value, column),
        _ => string.Empty,
    };

    private static string GetFormattedText(JsonNode? value, Column column) => value?.GetValue<string>() ?? column.NullValue;

    private static string GetFormattedDate(JsonNode? value, Column column)
    {
        if (value == null)
            return column.NullValue;
        _ = DateTime.TryParse(value.GetValue<string>(), out var castedValue);
        return castedValue.ToString("yyyy-MM-dd");
    }

    private static string GetFormattedPrecision(JsonNode? value, Column column)
    {
        if (value == null)
            return column.NullValue;
        return value.GetValue<decimal>().ToString(column.Format);
    }
}
