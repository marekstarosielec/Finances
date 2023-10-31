using FinancesBlazor.PropertyInfo;
using FinancesBlazor.ViewManager;
using System.Text.Json.Nodes;

namespace FinancesBlazor.Extensions;

public static class JsonArrayExtensions
{
    public static List<JsonNode?> GetDataForView(this JsonArray array, View? view)
    {
        var result = new List<JsonNode?>();

        if (array == null || view == null)
            return result;

        view.SortingColumnPropertyName ??= view.Properties.First().PropertyName;

        var dataType = view.Properties.FirstOrDefault(c => c.PropertyName == view.SortingColumnPropertyName)?.DataType;
        if (dataType == null)
            return result;

        var sortedNodes = JsonArrayDataTypeHandler.SortArray(array, view.Properties.First(c => c.PropertyName == view.SortingColumnPropertyName), view.SortingDescending);

        IEnumerable<JsonNode?>? filteredNodes = sortedNodes;
        foreach (var kvp in view.Filters)
            filteredNodes = JsonArrayDataTypeHandler.FitlerArray(filteredNodes, kvp.Key, kvp.Value);

        return filteredNodes?.Take(view.MaximumNumberOfRecords).ToList() ?? throw new InvalidOperationException("No records returned");
    }

    public static IOrderedEnumerable<JsonNode?> SortBy<TKey>(this JsonArray array, Func<JsonNode?, TKey> predicate, bool descending) => descending ? array.OrderByDescending(predicate) : array.OrderBy(predicate);
}
