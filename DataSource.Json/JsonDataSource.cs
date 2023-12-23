using System.Text;
using System.Text.Json.Nodes;

namespace DataSource.Json;

public class JsonDataSource : IDataSource
{
    private readonly string _fileName;
    protected static Dictionary<string, SemaphoreSlim> _semaphores = new ();
    private string? _contentString = null;
    public Dictionary<string, DataColumn> Columns { get; private set; }

    public JsonDataSource(string fileName, params DataColumn[] dataColumns)
    {
        if (string.IsNullOrWhiteSpace(fileName)) 
            throw new ArgumentException("FileName cannot be whitespace");
        
        _fileName = fileName;
        Columns = dataColumns.ToDictionary(c => c.ColumnName, c => c);
    }

    public async Task<DataQueryResult> ExecuteQuery(DataQuery? dataQuery = null) => TransformNodes(await GetNodes(dataQuery));

    internal async Task<NodesList> GetNodes(DataQuery? dataQuery)
    {
        var nodes = await GetData();
        var result = new NodesList(nodes);
        
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
        return result;
    }

    private async Task<IEnumerable<JsonNode>> GetData()
    {
        if (_contentString == null)
            await Load();
        return JsonNode.Parse(_contentString!)?.AsArray()?.Where(a => a != null)?.Select(a => a!) ?? throw new InvalidOperationException($"Failed to deserialize data from {_fileName}");
    }

    private async Task Load()
    {
        var _semaphore = GetSemaphore();

        try
        {
            _semaphore.Wait();
            if (!File.Exists(_fileName))
                await File.WriteAllTextAsync(_fileName, "[]");
            _contentString = await File.ReadAllTextAsync(_fileName, Encoding.Latin1);
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
        _contentString = null;
    }
}