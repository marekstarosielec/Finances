using FinancesBlazor.DataTypes;
using FinancesBlazor.ViewManager;
using System.Text.Json.Nodes;

namespace FinancesBlazor.Extensions;

public static class JsonArrayExtensions
{
    public static List<JsonNode?> FilterByParameters(this JsonArray array, ViewListParameters parameters)
    {
        var result = new List<JsonNode?>();

        if (array == null)
            return result;

        parameters ??= new ViewListParameters();
        parameters.SortingColumnDataName ??= parameters.Columns.First().Data;

        var dataType = parameters.Columns.FirstOrDefault(c => c.Data == parameters.SortingColumnDataName)?.DataType;
        if (dataType == null)
            return result;

        var sortedNodes = dataType switch
        {
            DataTypesList.Date or DataTypesList.Text => array.SortBy(n => n?[parameters.SortingColumnDataName]?.GetValue<string>(), parameters.SortingDescending),
            DataTypesList.Precision => array.SortBy(n => n?[parameters.SortingColumnDataName]?.GetValue<decimal>(), parameters.SortingDescending),
            _ => throw new InvalidOperationException(),
        };

        IEnumerable<JsonNode?>? filteredNodes = sortedNodes;
        foreach (var kvp in parameters.Filters)
        {
            switch (kvp.Key.DataType)
            {
                case DataTypesList.Text:
                    filteredNodes = filteredNodes.Where(n =>
                        (string.IsNullOrWhiteSpace(n?[kvp.Key.Data]?.GetValue<string?>()) && string.IsNullOrWhiteSpace(kvp.Value.StringValue))
                        || (n?[kvp.Key.Data]?.GetValue<string?>()?.ToLowerInvariant().Contains(kvp.Value.StringValue!.ToLowerInvariant()) == true)
                        );
                    break;
                case DataTypesList.Date:
                    filteredNodes = filteredNodes
                        .Select(n => new { Data = n, FilterData = n?[kvp.Key.Data]?.GetValue<DateTime?>() })
                        .Where(c => c.FilterData != null && c.FilterData >= kvp.Value.DateFrom && c.FilterData <= kvp.Value.DateTo)
                        .Select(c => c.Data);
                    break;
            }
        }
        return filteredNodes?.Take(parameters.MaximumNumberOfRecords).ToList() ?? throw new InvalidOperationException("No records returned");
    }

    public static IOrderedEnumerable<JsonNode?> SortBy<TKey>(this JsonArray array, Func<JsonNode?, TKey> predicate, bool descending) => descending ? array.OrderByDescending(predicate) : array.OrderBy(predicate);
}
