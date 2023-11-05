using System.Text.Json.Nodes;

namespace DataSource;

internal static class RowsExtensions
{
    internal static IEnumerable<Dictionary<DataColumn, object?>> Sort(this IEnumerable<Dictionary<DataColumn, object?>> source, DataColumn column, bool descending) => column.ColumnDataType switch
    {
        DataType.Date or DataType.Text
            => source.SortBy(n => n?[column] as string, descending),
        DataType.Precision
            => source.SortBy(n => n?[column] as decimal?, descending),
        DataType.Number
            => source.SortBy(n => n?[column] as int?, descending),
        _ => throw new InvalidOperationException(),
    };

    private static IEnumerable<Dictionary<DataColumn, object?>> SortBy<TKey>(this IEnumerable<Dictionary<DataColumn, object?>> source, Func<Dictionary<DataColumn, object?>, TKey> predicate, bool descending) => descending ? source.OrderByDescending(predicate) : source.OrderBy(predicate);

    internal static IEnumerable<Dictionary<DataColumn, object?>> Fitler(this IEnumerable<Dictionary<DataColumn, object?>> source, DataColumn column, DataColumnFilter filter) => column.ColumnDataType switch
    {
        DataType.Text =>
            source.Where(n =>
                (string.IsNullOrWhiteSpace(n?[column] as string) && string.IsNullOrWhiteSpace(filter.StringValue))
                || ((n?[column] as string)?.ToLowerInvariant().Contains(filter.StringValue!.ToLowerInvariant()) == true)
                ),
        DataType.Date =>
            source
                .Select(n => new { Data = n, FilterData = n?[column] as DateTime? })
                .Where(c => c.FilterData != null && c.FilterData >= filter.DateFrom && c.FilterData <= filter.DateTo)
                .Select(c => c.Data),
        DataType.Precision
            => throw new InvalidOperationException(),
        DataType.Number
            => throw new InvalidOperationException(),
        _ => throw new InvalidOperationException()
    };
}
