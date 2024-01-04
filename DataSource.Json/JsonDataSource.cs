using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace DataSource.Json;

public class JsonDataSource : IDataSource
{
    private readonly string _fileName;
    protected static Dictionary<string, SemaphoreSlim> _semaphores = new ();
    public Dictionary<string, DataColumn> Columns { get; private set; }
    public DataSourceCache<NodesList> Cache { get; } = new ();

    public DateTime? CacheTimeStamp => Cache.TimeStamp;

    public JsonDataSource(string fileName, params DataColumn[] dataColumns)
    {
        if (string.IsNullOrWhiteSpace(fileName)) 
            throw new ArgumentException("FileName cannot be whitespace");
        
        _fileName = fileName;
        Columns = dataColumns.ToDictionary(c => c.ColumnName, c => c);
    }

    public async Task<DataQueryResult> ExecuteQuery(DataQuery? dataQuery = null) => TransformNodes(await GetNodes(dataQuery));

    public async Task Save(DataRow row)
    {
        var nodes = await GetData();
        var id = row[Columns["Id"]].OriginalValue as string;
        var changedNode = nodes.Nodes.FirstOrDefault(n => n["Id"]?.GetValue<string>() == id)?.AsObject();
        if (changedNode == null)
            return; //TODO: New row created;
        foreach (var cell in row)
        {
            if (cell.Value.CurrentValue == cell.Value.OriginalValue)
                continue;

            changedNode[cell.Key.ColumnName] = JsonValue.Create(cell.Value.CurrentValue);
        }

        var result = JsonSerializer.Serialize(nodes);
        var _semaphore = GetSemaphore();

        try
        {
            _semaphore.Wait();
            if (!File.Exists(_fileName))
                await File.WriteAllTextAsync(_fileName, "[]");
            await File.WriteAllTextAsync(_fileName, result);
        }
        catch(Exception ex)
        {
            //TODO: restore data?
            return;
        }
        finally
        {
            _semaphore.Release();
        }
        //TODO: updating transaction need to invalidate cache of transactionwithdocument
        Cache.Clean();
        await GetData();
    }

    internal async Task<NodesList> GetNodes(DataQuery? dataQuery)
    {
        var result = await GetData();
        
        if (dataQuery?.Sorters != null)
            foreach (var sortDefinition in dataQuery.Sorters)
                result.Nodes = result.Nodes.Sort(sortDefinition.Key, sortDefinition.Value);

        if (dataQuery?.Filters != null)
            foreach (var filterDefinition in dataQuery.Filters)
                result.Nodes = result.Nodes.Fitler(filterDefinition.Key, filterDefinition.Value);

        result.Count = result.Nodes.Count();

        if (dataQuery?.PageSize.GetValueOrDefault(-1) > -1)
            result.Nodes = result.Nodes.Take(dataQuery.PageSize!.Value);

        return result;
    }

    private DataQueryResult TransformNodes(NodesList nodes)
    {
        var rows = new List<DataRow>();
        var result = new DataQueryResult(Columns.Values, rows, nodes.Count);
        var watch = System.Diagnostics.Stopwatch.StartNew();
        foreach (var node in nodes.Nodes)
        {
            var row = new DataRow();
            foreach (var column in Columns)
                try
                {
                    row[column.Value] = new DataValue(column.Value.ColumnDataType switch
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
            rows.Add(row);
        }
        Console.WriteLine($"Query processing: {watch.ElapsedMilliseconds} ms");
        return result;
    }

    private async Task<NodesList> GetData() => await Cache.Get(Load);

    private async Task<NodesList> Load()
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
            var result = JsonNode.Parse(json)?.AsArray()?.Where(a => a != null)?.Select(a => a!);
            if (result == null)
                throw new InvalidOperationException("Input json evaluated to null");
            Console.WriteLine($"Json file parsing: {watch.ElapsedMilliseconds} ms");
            return new NodesList(result);
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