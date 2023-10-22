using System.Text.Json.Nodes;

namespace FinancesBlazor.Components.Grid;

public static class GridValueFormatter
{
    public static string GetFormattedString(JsonNode? value, GridColumn gridColumn) => gridColumn.DataType switch
    {
        DataAccess.DataTypes.Text => GetFormattedText(value, gridColumn),
        DataAccess.DataTypes.Date => GetFormattedDate(value, gridColumn),
        DataAccess.DataTypes.Precision => GetFormattedPrecision(value, gridColumn),
        _ => string.Empty,
    };

    private static string GetFormattedText(JsonNode? value, GridColumn gridColumn) => value?.GetValue<string>() ?? gridColumn.NullValue;

    private static string GetFormattedDate(JsonNode? value, GridColumn gridColumn)
    {
        if (value == null)
            return gridColumn.NullValue;
        _ = DateTime.TryParse(value.GetValue<string>(), out var castedValue);
        return castedValue.ToString("yyyy-MM-dd");
    }

    private static string GetFormattedPrecision(JsonNode? value, GridColumn gridColumn)
    {
        if (value == null)
            return gridColumn.NullValue;
        return value.GetValue<decimal>().ToString(gridColumn.Format);
    }
}
