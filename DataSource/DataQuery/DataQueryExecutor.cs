using System.Collections;

namespace DataSource;

internal class DataQueryExecutor
{
    public async Task<DataQueryResult> ExecuteQuery(string id, DataQuery dataQuery)
    {
        Console.WriteLine($"{id}: Execute query");
        var allData = await DataSourceCache.Instance.Get(id);
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
        var result = new DataQueryResult(validColumns, validRows, count);
        await AddGroupColumn(result);
        return result;
    }

    /// <summary>
    /// Get related group data, if group column was added to data source.
    /// </summary>
    /// <param name="dataQueryResult"></param>
    /// <param name="groupStamp"></param>
    public async Task AddGroupColumn(DataQueryResult dataQueryResult)
    {
        if (!dataQueryResult.Columns.Any(dc => GroupDataColumn.IsGroupColumn(dc)))
            return;

        var allData = await DataSourceCache.Instance.Get(GroupDataSource.Id);
        foreach (var row in dataQueryResult.Rows)
        {
            row[GroupDataColumn.Name] = new DataValue(null);
            var groupId = allData.Rows.Filter(GroupDataSource.RowIdDataColumn, new DataColumnFilter { StringValue = new List<string> { (string) row.Id.CurrentValue! } })?.FirstOrDefault()?[GroupDataSource.GroupIdDataColumn.ColumnName].CurrentValue as string;
            if (groupId == null)
                continue;

            var groupData = allData.Rows.Filter(GroupDataSource.GroupIdDataColumn, new DataColumnFilter { StringValue = new List<string> { groupId } });
            row[GroupDataColumn.Name] = new DataValue(new GroupDataValue(groupId, groupData));
            
        }
    }
}
