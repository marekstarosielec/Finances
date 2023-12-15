using System.Text.Json.Nodes;

namespace DataSource;

internal static class RowsExtensions
{
    internal static IEnumerable<Dictionary<DataColumn, object?>> Sort(this IEnumerable<Dictionary<DataColumn, object?>> source, DataColumn column, bool descending) => column.ColumnDataType switch
    {
        ColumnDataType.Text
            => source.SortBy(n => n?[column] as string, descending),
        ColumnDataType.Date
            => source.SortBy(n => n?[column] as DateTime?, descending),
        ColumnDataType.Precision
            => source.SortBy(n => n?[column] as decimal?, descending),
        ColumnDataType.Number
            => source.SortBy(n => n?[column] as int?, descending),
        _ => throw new InvalidOperationException(),
    };

    private static IEnumerable<Dictionary<DataColumn, object?>> SortBy<TKey>(this IEnumerable<Dictionary<DataColumn, object?>> source, Func<Dictionary<DataColumn, object?>, TKey> predicate, bool descending) => descending ? source.OrderByDescending(predicate) : source.OrderBy(predicate);

    internal static IEnumerable<Dictionary<DataColumn, object?>> Filter(this IEnumerable<Dictionary<DataColumn, object?>> source, DataColumn column, DataColumnFilter filter) => (column.ColumnDataType, filter.Equality) switch
    {
        (ColumnDataType.Text, Equality.Equals) =>
            source.Where(n => filter.StringValue == null || (n?[column] as string)?.ToLowerInvariant() == filter.StringValue!.ToLowerInvariant()),
        (ColumnDataType.Text, Equality.NotEquals) =>
            source.Where(n => filter.StringValue == null || (n?[column] as string)?.ToLowerInvariant() != filter.StringValue!.ToLowerInvariant()),
        (ColumnDataType.Text, Equality.Contains) =>
            source.Where(n => filter.StringValue == null || (n?[column] as string)?.ToLowerInvariant().Contains(filter.StringValue.ToLowerInvariant()) == true),
        (ColumnDataType.Date, _) =>
            source
                .Select(n => new { Data = n, FilterData = n?[column] as DateTime? })
                .Where(c => c.FilterData != null && c.FilterData >= filter.DateFrom && c.FilterData <= filter.DateTo)
                .Select(c => c.Data),
        (ColumnDataType.Precision, _)
            => throw new InvalidOperationException(),
        (ColumnDataType.Number, _)
            => throw new InvalidOperationException(),
        (_, _) => throw new InvalidOperationException()
    };
}
