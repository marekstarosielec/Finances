namespace DataSource;

internal static class DataRowsExtensions
{
    internal static IEnumerable<DataRow> Sort(this IEnumerable<DataRow> source, DataColumn column, bool descending) => column.ColumnDataType switch
    {
        ColumnDataType.Text
            => source.SortBy(n => n?[column].OriginalValue as string, descending),
        ColumnDataType.Date
            => source.SortBy(n => n?[column].OriginalValue as DateTime?, descending),
        ColumnDataType.Precision
            => source.SortBy(n => n?[column].OriginalValue as decimal?, descending),
        ColumnDataType.Number
            => source.SortBy(n => n?[column].OriginalValue as int?, descending),
        _ => throw new InvalidOperationException(),
    };

    private static IEnumerable<DataRow> SortBy<TKey>(this IEnumerable<DataRow> source, Func<DataRow, TKey> predicate, bool descending) => descending ? source.OrderByDescending(predicate) : source.OrderBy(predicate);

    internal static IEnumerable<DataRow> Filter(this IEnumerable<DataRow> source, DataColumn column, DataColumnFilter filter) => (column.ColumnDataType, filter.Equality) switch
    {
        (ColumnDataType.Text, Equality.Equals) =>
            source.Where(n => filter.StringValue.Count == 0 || filter.StringValue.Any(s => s == (n?[column].OriginalValue as string)?.ToLowerInvariant())),
        (ColumnDataType.Text, Equality.NotEquals) =>
            source.Where(n => filter.StringValue.Count == 0 || !filter.StringValue.Any(s => s == (n?[column].OriginalValue as string)?.ToLowerInvariant())),
        (ColumnDataType.Text, Equality.Contains) =>
            source.Where(n => filter.StringValue.Count == 0 || filter.StringValue.Any(s => (n?[column].OriginalValue as string)?.ToLowerInvariant().Contains(s) ?? false)),
        (ColumnDataType.Date, _) =>
            source
                .Select(n => new { Data = n, FilterData = n?[column].OriginalValue as DateTime? })
                .Where(c => c.FilterData != null && c.FilterData >= filter.DateFrom && c.FilterData <= filter.DateTo)
                .Select(c => c.Data),
        (ColumnDataType.Precision, _)
            => throw new InvalidOperationException(),
        (ColumnDataType.Number, _)
            => throw new InvalidOperationException(),
        (_, _) => throw new InvalidOperationException()
    };
}
