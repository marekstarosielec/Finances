using FinancesBlazor.Extensions;
using FinancesBlazor.ViewManager;
using System.Text.Json.Nodes;

namespace FinancesBlazor.PropertyInfo;

public static class JsonArrayDataTypeHandler
{
    public static IOrderedEnumerable<JsonNode?> SortArray(JsonArray array, PropertyInfoBase sortingProperty, bool sortingDescending) => sortingProperty.DataType switch
    {
        DataType.Date or DataType.Text => array.SortBy(n => n?[sortingProperty.PropertyName]?.GetValue<string>(), sortingDescending),
        DataType.Precision => array.SortBy(n => n?[sortingProperty.PropertyName]?.GetValue<decimal>(), sortingDescending),
        _ => throw new InvalidOperationException(),
    };

    public static IEnumerable<JsonNode?> FitlerArray(IEnumerable<JsonNode?> array, PropertyInfoBase filterProperty, FilterInfoBase filterInfo) => filterProperty.DataType switch
    {
        DataType.Text =>
            array.Where(n =>
                (string.IsNullOrWhiteSpace(n?[filterProperty.PropertyName]?.GetValue<string?>()) && string.IsNullOrWhiteSpace(filterInfo.StringValue))
                || (n?[filterProperty.PropertyName]?.GetValue<string?>()?.ToLowerInvariant().Contains(filterInfo.StringValue!.ToLowerInvariant()) == true)
                ),
        DataType.Date =>
            array
                .Select(n => new { Data = n, FilterData = n?[filterProperty.PropertyName]?.GetValue<DateTime?>() })
                .Where(c => c.FilterData != null && c.FilterData >= filterInfo.DateFrom && c.FilterData <= filterInfo.DateTo)
                .Select(c => c.Data),
        DataType.Precision => throw new InvalidOperationException(),
        _ => throw new InvalidOperationException()
    };
}
