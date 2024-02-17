namespace DataSource;

internal class DataQueryExecutor
{
    public async Task<DataQueryResult> ExecuteQuery(string id, DataSourceCacheStamp cacheStamp, DataQuery dataQuery)
    {
        var allData = await DataSourceCache.Instance.Get(id, cacheStamp);
        var clonedData = allData.Clone();
        var clonedRows = clonedData.Rows;

        if (dataQuery.Filters != null)
            foreach (var filterDefinition in dataQuery.Filters)
                clonedRows = clonedRows.Filter(filterDefinition.Key, filterDefinition.Value).ToList();

        clonedRows = clonedRows.Sort(dataQuery.Sorters);

        var count = clonedRows.Count();

        if (dataQuery.PageSize.GetValueOrDefault(-1) > -1)
            clonedRows = clonedRows.Take(dataQuery.PageSize!.Value);

        //Remove columns (and its data) that are not specified in dataQuery. If no columns are specified (before joining or unioning tables), return all columns.
        var validColumns = clonedData.Columns.Where(c => dataQuery.Columns.Count == 0 || dataQuery.Columns.Any(dq => dq.ColumnName == c.ColumnName));
        var validRows = new List<DataRow>();
        foreach (var clonedRow in clonedRows)
        {
            var validRow = new DataRow();
            foreach (var dataColumn in validColumns)
                if (clonedRow.ContainsKey(dataColumn.ColumnName)) //Columns that are generated (e.g. Group) are not available here
                    validRow[dataColumn.ColumnName] = clonedRow[dataColumn.ColumnName];
            validRows.Add(validRow);
        }

        return new DataQueryResult(validColumns, validRows, count);
    }
}
