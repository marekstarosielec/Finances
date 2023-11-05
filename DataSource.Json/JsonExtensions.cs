using System.Text.Json.Nodes;

namespace DataSource.Json;

internal static class JsonExtensions
{
    internal static IEnumerable<JsonNode> Sort(this IEnumerable<JsonNode> source, DataColumn column, bool descending) => column.ColumnDataType switch
    {
        DataType.Date or DataType.Text 
            => source.SortBy(n => n?[column.ColumnName]?.GetValue<string>(), descending),
        DataType.Precision 
            => source.SortBy(n => n?[column.ColumnName]?.GetValue<decimal>(), descending),
        DataType.Number
            => source.SortBy(n => n?[column.ColumnName]?.GetValue<int>(), descending),
        _ => throw new InvalidOperationException(),
    };

    internal static IEnumerable<JsonNode> Fitler(this IEnumerable<JsonNode> source, DataColumn column, DataColumnFilter filter) => column.ColumnDataType switch
    {
        DataType.Text =>
            source.Where(n =>
                (string.IsNullOrWhiteSpace(n?[column.ColumnName]?.GetValue<string?>()) && string.IsNullOrWhiteSpace(filter.StringValue))
                || (n?[column.ColumnName]?.GetValue<string?>()?.ToLowerInvariant().Contains(filter.StringValue!.ToLowerInvariant()) == true)
                ),
        DataType.Date =>
            source
                .Select(n => new { Data = n, FilterData = n?[column.ColumnName]?.GetValue<DateTime?>() })
                .Where(c => c.FilterData != null && c.FilterData >= filter.DateFrom && c.FilterData <= filter.DateTo)
                .Select(c => c.Data),
        DataType.Precision
            => throw new InvalidOperationException(),
        DataType.Number
            => throw new InvalidOperationException(),
        _ => throw new InvalidOperationException()
    };

    private static IEnumerable<JsonNode> SortBy<TKey>(this IEnumerable<JsonNode> array, Func<JsonNode?, TKey> predicate, bool descending) => descending ? array.OrderByDescending(predicate) : array.OrderBy(predicate);
}
