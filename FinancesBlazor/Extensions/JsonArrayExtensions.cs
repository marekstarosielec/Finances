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

        var sortedNodes = dataType switch
        {
            //TODO: Extract to PropertyInfo
            DataType.Date or DataType.Text => array.SortBy(n => n?[view.SortingColumnPropertyName]?.GetValue<string>(), view.SortingDescending),
            DataType.Precision => array.SortBy(n => n?[view.SortingColumnPropertyName]?.GetValue<decimal>(), view.SortingDescending),
            _ => throw new InvalidOperationException(),
        };

        IEnumerable<JsonNode?>? filteredNodes = sortedNodes;
        foreach (var kvp in view.Filters)
        {
            //TODO: Extract to PropertyInfo
            switch (kvp.Key.DataType)
            {
                case DataType.Text:
                    filteredNodes = filteredNodes.Where(n =>
                        (string.IsNullOrWhiteSpace(n?[kvp.Key.PropertyName]?.GetValue<string?>()) && string.IsNullOrWhiteSpace(kvp.Value.StringValue))
                        || (n?[kvp.Key.PropertyName]?.GetValue<string?>()?.ToLowerInvariant().Contains(kvp.Value.StringValue!.ToLowerInvariant()) == true)
                        );
                    break;
                case DataType.Date:
                    filteredNodes = filteredNodes
                        .Select(n => new { Data = n, FilterData = n?[kvp.Key.PropertyName]?.GetValue<DateTime?>() })
                        .Where(c => c.FilterData != null && c.FilterData >= kvp.Value.DateFrom && c.FilterData <= kvp.Value.DateTo)
                        .Select(c => c.Data);
                    break;
            }
        }
        return filteredNodes?.Take(view.MaximumNumberOfRecords).ToList() ?? throw new InvalidOperationException("No records returned");
    }

    public static IOrderedEnumerable<JsonNode?> SortBy<TKey>(this JsonArray array, Func<JsonNode?, TKey> predicate, bool descending) => descending ? array.OrderByDescending(predicate) : array.OrderBy(predicate);
}
