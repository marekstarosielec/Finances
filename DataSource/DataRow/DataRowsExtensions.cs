using System.Data;

namespace DataSource;

public static class DataRowsExtensions
{
    public static IEnumerable<DataRow> Sort(this IEnumerable<DataRow> source, Dictionary<DataColumn, bool>? sorters)
    {
        if (sorters == null || sorters.Count == 0)
            return source;
        var firstDataColumn = sorters.FirstOrDefault().Key;
        var result = source.OrderBy(s => 1) ?? throw new InvalidCastException("Failed to case list to IOrderedEnumerable");
        foreach (var dataColumn in sorters.Keys)
        {
            result = dataColumn.ColumnDataType switch
            {
                ColumnDataType.Text
                    => result.SortBy(dataColumn == firstDataColumn, n => n?[dataColumn.ColumnName].OriginalValue as string, sorters[dataColumn]),
                ColumnDataType.Date
                    => result.SortBy(dataColumn == firstDataColumn, n => n?[dataColumn.ColumnName].OriginalValue as DateTime?, sorters[dataColumn]),
                ColumnDataType.Precision
                    => result.SortBy(dataColumn == firstDataColumn, n => n?[dataColumn.ColumnName].OriginalValue as decimal?, sorters[dataColumn]),
                ColumnDataType.Number
                    => result.SortBy(dataColumn == firstDataColumn, n => n?[dataColumn.ColumnName].OriginalValue as int?, sorters[dataColumn]),
                ColumnDataType.Bool
                    => result.SortBy(dataColumn == firstDataColumn, n => n?[dataColumn.ColumnName].OriginalValue as bool?, sorters[dataColumn]),
                _ => throw new InvalidOperationException(),
            };
        }
        return result;
    }

    private static IOrderedEnumerable<DataRow> SortBy<TKey>(this IOrderedEnumerable<DataRow> source, bool IsFirstSort, Func<DataRow, TKey> predicate, bool descending) => IsFirstSort ? FirstSortBy(source, predicate, descending) : ThenSortBy(source, predicate, descending);

    private static IOrderedEnumerable<DataRow> FirstSortBy<TKey>(this IEnumerable<DataRow> source, Func<DataRow, TKey> predicate, bool descending) => descending ? source.OrderByDescending(predicate) : source.OrderBy(predicate);
    private static IOrderedEnumerable<DataRow> ThenSortBy<TKey>(this IOrderedEnumerable<DataRow> source, Func<DataRow, TKey> predicate, bool descending) => descending ? source.ThenByDescending(predicate) : source.ThenBy(predicate);

    public static IEnumerable<DataRow> Filter(this IEnumerable<DataRow> source, DataColumn column, DataColumnFilter filter) => (column.ColumnDataType, filter.Equality) switch
    {
        (ColumnDataType.Text, Equality.Equals) =>
            source.Where(n => filter.StringValue.Count == 0 || filter.StringValue.Any(s => s.ToLowerInvariant() == (n?[column.ColumnName].OriginalValue as string)?.ToLowerInvariant())),
        (ColumnDataType.Text, Equality.NotEquals) =>
            source.Where(n => filter.StringValue.Count == 0 || !filter.StringValue.Any(s => s.ToLowerInvariant() == (n?[column.ColumnName].OriginalValue as string)?.ToLowerInvariant())),
        (ColumnDataType.Text, Equality.Contains) =>
            source.Where(n => filter.StringValue.Count == 0 || filter.StringValue.Any(s => (n?[column.ColumnName].OriginalValue as string)?.ToLowerInvariant().Contains(s.ToLowerInvariant()) ?? false)),
        (ColumnDataType.Date, _) =>
            source
                .Select(n => new { Data = n, FilterData = n?[column.ColumnName].OriginalValue as DateTime? })
                .Where(c => c.FilterData != null && c.FilterData >= filter.DateFrom && c.FilterData <= filter.DateTo)
                .Select(c => c.Data),
        (ColumnDataType.Precision, _)
            => throw new InvalidOperationException(),
        (ColumnDataType.Number, _)
            => throw new InvalidOperationException(),
        (ColumnDataType.Bool, _)
            => throw new InvalidOperationException(),
        (_, _) => throw new InvalidOperationException()
    };
}
