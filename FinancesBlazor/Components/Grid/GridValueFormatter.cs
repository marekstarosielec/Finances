namespace FinancesBlazor.Components.Grid;

public static class GridValueFormatter
{
    public static string GetFormattedString(object? value, GridColumn gridColumn) => gridColumn.DataType switch
    {
        DataAccess.DataTypes.Text => GetFormattedText(value, gridColumn),
        DataAccess.DataTypes.Date => GetFormattedDate(value, gridColumn),
        DataAccess.DataTypes.Precision => GetFormattedPrecision(value, gridColumn),
        _ => string.Empty,
    };

    private static string GetFormattedText(object? value, GridColumn gridColumn) => value?.ToString() ?? gridColumn.NullValue;

    private static string GetFormattedDate(object? value, GridColumn gridColumn)
    {
        var castedValue = (DateTime?)value;
        return castedValue.HasValue ? castedValue.Value.ToString("yyyy-MM-dd") : gridColumn.NullValue;
    }

    private static string GetFormattedPrecision(object? value, GridColumn gridColumn)
    {
        var castedValue = (double?)value;
        return castedValue.HasValue ? castedValue.Value.ToString(gridColumn.Format) : gridColumn.NullValue;
    }
}
