using System.Text.Json.Nodes;

namespace DataSource.Json;

internal static class JsonExtensions
{
    internal static IEnumerable<JsonNode> Sort(this IEnumerable<JsonNode> source, DataColumn column, bool descending) => column.ColumnDataType switch
    {
        ColumnDataType.Date or ColumnDataType.Text 
            => source.SortBy(n => n?[column.ColumnName]?.GetValue<string>(), descending),
        ColumnDataType.Precision 
            => source.SortBy(n => n?[column.ColumnName]?.GetValue<decimal>(), descending),
        ColumnDataType.Number
            => source.SortBy(n => n?[column.ColumnName]?.GetValue<int>(), descending),
        _ => throw new InvalidOperationException(),
    };

    private static IEnumerable<JsonNode> SortBy<TKey>(this IEnumerable<JsonNode> array, Func<JsonNode?, TKey> predicate, bool descending) => descending ? array.OrderByDescending(predicate) : array.OrderBy(predicate);

    internal static IEnumerable<JsonNode> Fitler(this IEnumerable<JsonNode> source, DataColumn column, DataColumnFilter filter) => column.ColumnDataType switch
    {
        ColumnDataType.Text =>
            source.Where(n =>
                (string.IsNullOrWhiteSpace(n?[column.ColumnName]?.GetValue<string?>()) && filter.StringValue.Count == 0)
                || (filter.StringValue.Any(s => n?[column.ColumnName]?.GetValue<string?>()?.ToLowerInvariant().Contains(s.ToLowerInvariant()) == true))),
        ColumnDataType.Date =>
            source
                .Select(n => new { Data = n, FilterData = n?[column.ColumnName]?.GetValue<DateTime?>() })
                .Where(c => c.FilterData != null && c.FilterData >= filter.DateFrom && c.FilterData <= filter.DateTo)
                .Select(c => c.Data),
        ColumnDataType.Precision
            => throw new InvalidOperationException(),
        ColumnDataType.Number
            => throw new InvalidOperationException(),
        _ => throw new InvalidOperationException()
    };
}
