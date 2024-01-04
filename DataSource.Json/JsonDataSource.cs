using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace DataSource.Json;

public class JsonDataSource : IDataSource
{
    private readonly string _fileName;
    protected static Dictionary<string, SemaphoreSlim> _semaphores = new ();
    public Dictionary<string, DataColumn> Columns { get; private set; }
    public DataSourceCache<DataQueryResult> Cache { get; } = new ();

    public DateTime? CacheTimeStamp => Cache.TimeStamp;

    public JsonDataSource(string fileName, params DataColumn[] dataColumns)
    {
        if (string.IsNullOrWhiteSpace(fileName)) 
            throw new ArgumentException("FileName cannot be whitespace");
        
        _fileName = fileName;
        Columns = dataColumns.ToDictionary(c => c.ColumnName, c => c);
    }

    public async Task<DataQueryResult> ExecuteQuery(DataQuery? dataQuery = null)
    {
        {
            var allData = await GetAllData();
            var clonedData = allData.Clone();
            var clonedRows = clonedData.Rows;

            clonedRows = clonedRows.Sort(dataQuery.Sorters);

            if (dataQuery?.Filters != null)
                foreach (var filterDefinition in dataQuery.Filters)
                    clonedRows = clonedRows.Filter(filterDefinition.Key, filterDefinition.Value).ToList();

            var count = clonedRows.Count();

            if (dataQuery?.PageSize.GetValueOrDefault(-1) > -1)
                clonedRows = clonedRows.Take(dataQuery.PageSize!.Value);

            return new DataQueryResult(clonedData.Columns, clonedRows, count);
        }
    }

    public async Task Save(DataRow row)
    {
        var allData = await GetAllData();
        var id = row["Id"].OriginalValue as string;
       // var changedRow = allData.Rows.FirstOrDefault(r => r["Id"]?.GetValue<string>() == id)?.AsObject();
        //if (changedNode == null)
        //    return; //TODO: New row created;
        //foreach (var cell in row)
        //{
        //    if (cell.Value.CurrentValue == cell.Value.OriginalValue)
        //        continue;

        //    changedNode[cell.Key.ColumnName] = JsonValue.Create(cell.Value.CurrentValue);
        //}

        //var result = JsonSerializer.Serialize(nodes);
        //var _semaphore = GetSemaphore();

        //try
        //{
        //    _semaphore.Wait();
        //    if (!File.Exists(_fileName))
        //        await File.WriteAllTextAsync(_fileName, "[]");
        //    await File.WriteAllTextAsync(_fileName, result);
        //}
        //catch(Exception ex)
        //{
        //    //TODO: restore data?
        //    return;
        //}
        //finally
        //{
        //    _semaphore.Release();
        //}
        ////TODO: updating transaction need to invalidate cache of transactionwithdocument
        //Cache.Clean();
        //await GetData();
    }

    private async Task<DataQueryResult> GetAllData() => await Cache.Get(Load);

    private async Task<DataQueryResult> Load()
    {
        var _semaphore = GetSemaphore();

        try
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _semaphore.Wait();
            if (!File.Exists(_fileName))
                await File.WriteAllTextAsync(_fileName, "[]");
            var json = await File.ReadAllTextAsync(_fileName, Encoding.Latin1);
            if (string.IsNullOrWhiteSpace(json))
                throw new InvalidOperationException("Input json evaluated to null");
            Console.WriteLine($"Json file loading: {watch.ElapsedMilliseconds} ms");

            watch.Restart();
            var nodes = JsonNode.Parse(json)?.AsArray()?.Where(a => a != null)?.Select(a => a!)?.ToList();
            if (nodes == null)
                throw new InvalidOperationException("Input json evaluated to null");
            Console.WriteLine($"Json file parsing: {watch.ElapsedMilliseconds} ms");

            var dataRows = new List<DataRow>();
            foreach (var node in nodes)
            {
                var dataRow = new DataRow();
                foreach (var column in Columns)
                    try
                    {
                        dataRow[column.Value.ColumnName] = new DataValue(column.Value.ColumnDataType switch
                        {
                            ColumnDataType.Text => node[column.Key]?.GetValue<string>(),
                            ColumnDataType.Date => node[column.Key]?.GetValue<DateTime>(),
                            ColumnDataType.Precision => node[column.Key]?.GetValue<decimal>(),
                            ColumnDataType.Number => node[column.Key]?.GetValue<int>(),
                            _ => throw new Exception("Unsupported DataType")
                        });
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidCastException($"Failed to convert {column.Key}", ex);
                    }
                dataRows.Add(dataRow);
            }

            return new DataQueryResult(Columns.Values, dataRows, dataRows.Count);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private SemaphoreSlim GetSemaphore()
    {
        if (!_semaphores.ContainsKey(_fileName))
            _semaphores[_fileName] = new SemaphoreSlim(1); 
        return _semaphores[_fileName];
    }

    public void RemoveCache()
    {
        Cache.Clean();
    }

    
}