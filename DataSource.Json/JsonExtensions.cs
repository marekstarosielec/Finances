using System.Text.Json.Nodes;

namespace DataSource.Json;

public static class JsonExtensions
{
    public static IEnumerable<JsonNode> Sort(this IEnumerable<JsonNode> source, DataColumn column, bool descending) => column.ColumnDataType switch
    {
        DataType.Date or DataType.Text 
            => source.SortBy(n => n.GetDeepNode(column)?[column.DeepColumnName]?.GetValue<string>(), descending),
        DataType.Precision 
            => source.SortBy(n => n.GetDeepNode(column)?[column.DeepColumnName]?.GetValue<decimal>(), descending),
        _ => throw new InvalidOperationException(),
    };

    public static IEnumerable<JsonNode> Fitler(this IEnumerable<JsonNode> source, DataColumn column, DataColumnFilter filter) => column.ColumnDataType switch
    {
        DataType.Text =>
            source.Where(n =>
                (string.IsNullOrWhiteSpace(n.GetDeepNode(column)?[column.DeepColumnName]?.GetValue<string?>()) && string.IsNullOrWhiteSpace(filter.StringValue))
                || (n.GetDeepNode(column)?[column.DeepColumnName]?.GetValue<string?>()?.ToLowerInvariant().Contains(filter.StringValue!.ToLowerInvariant()) == true)
                ),
        DataType.Date =>
            source
                .Select(n => new { Data = n, FilterData = n.GetDeepNode(column)?[column.DeepColumnName]?.GetValue<DateTime?>() })
                .Where(c => c.FilterData != null && c.FilterData >= filter.DateFrom && c.FilterData <= filter.DateTo)
                .Select(c => c.Data),
        DataType.Precision
            => throw new InvalidOperationException(),
        _ => throw new InvalidOperationException()
    };

    private static IEnumerable<JsonNode> SortBy<TKey>(this IEnumerable<JsonNode> array, Func<JsonNode?, TKey> predicate, bool descending) => descending ? array.OrderByDescending(predicate) : array.OrderBy(predicate);

    private static JsonNode? GetDeepNode(this JsonNode? value, DataColumn column)
    {
        var result = value;
        var subColumn = column.ColumnName.Split('.');
        for (int i = 0; i < subColumn.Length - 1; i++)
            if (result != null)
                result = result[subColumn[i]];
        return result;
    }
}
