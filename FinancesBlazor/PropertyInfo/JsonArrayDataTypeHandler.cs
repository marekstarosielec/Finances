using FinancesBlazor.Extensions;
using FinancesBlazor.ViewManager;
using System.Text.Json.Nodes;

namespace FinancesBlazor.PropertyInfo;

public static class JsonArrayDataTypeHandler
{
    public static IOrderedEnumerable<JsonNode?> SortArray(JsonArray array, PropertyInfoBase sortingProperty, bool sortingDescending) => sortingProperty.DataType switch
    {
        DataType.Date or DataType.Text => array.SortBy(n => n.GetDeepNode(sortingProperty)?[sortingProperty.DeepPropertyName]?.GetValue<string>(), sortingDescending),
        DataType.Precision or DataType.Money => array.SortBy(n => n.GetDeepNode(sortingProperty)?[sortingProperty.DeepPropertyName]?.GetValue<decimal>(), sortingDescending),
        _ => throw new InvalidOperationException(),
    };

    public static IEnumerable<JsonNode?> FitlerArray(IEnumerable<JsonNode?> array, PropertyInfoBase filterProperty, FilterInfoBase filterInfo) => filterProperty.DataType switch
    {
        DataType.Text =>
            array.Where(n =>
                (string.IsNullOrWhiteSpace(n.GetDeepNode(filterProperty)?[filterProperty.DeepPropertyName]?.GetValue<string?>()) && string.IsNullOrWhiteSpace(filterInfo.StringValue))
                || (n.GetDeepNode(filterProperty)?[filterProperty.DeepPropertyName]?.GetValue<string?>()?.ToLowerInvariant().Contains(filterInfo.StringValue!.ToLowerInvariant()) == true)
                ),
        DataType.Date =>
            array
                .Select(n => new { Data = n, FilterData = n.GetDeepNode(filterProperty)?[filterProperty.DeepPropertyName]?.GetValue<DateTime?>() })
                .Where(c => c.FilterData != null && c.FilterData >= filterInfo.DateFrom && c.FilterData <= filterInfo.DateTo)
                .Select(c => c.Data),
        DataType.Precision or DataType.Money => throw new InvalidOperationException(),
        _ => throw new InvalidOperationException()
    };
}
