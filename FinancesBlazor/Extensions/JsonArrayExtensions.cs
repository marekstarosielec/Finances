using FinancesBlazor.DataAccess;
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

        result = array.ToList();
        foreach (var kvp in parameters.Filters)
        {
            switch (kvp.Key.DataType)
            {
                case DataTypes.Text:
                    result = result.Where(n =>
                        (string.IsNullOrWhiteSpace(n?[kvp.Key.Data]?.GetValue<string?>()) && string.IsNullOrWhiteSpace(kvp.Value.StringValue))
                        || (n?[kvp.Key.Data]?.GetValue<string?>()?.ToLowerInvariant().Contains(kvp.Value.StringValue!.ToLowerInvariant()) == true)
                        ).ToList();
                    break;
            }
        }

        return dataType switch
        {
            DataTypes.Date or DataTypes.Text => result.SortBy(n => n?[parameters.SortingColumnDataName]?.GetValue<string>(), parameters.SortingDescending).ToList(),
            DataTypes.Precision => result.SortBy(n => n?[parameters.SortingColumnDataName]?.GetValue<decimal>(), parameters.SortingDescending).ToList(),
            _ => throw new InvalidOperationException(),
        };
    }

    public static List<JsonNode?> SortBy<TKey>(this List<JsonNode?> array, Func<JsonNode?, TKey> predicate, bool descending) => descending ? array.OrderByDescending(predicate).ToList() : array.OrderBy(predicate).ToList();
}
