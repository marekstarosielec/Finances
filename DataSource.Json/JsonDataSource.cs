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

    public async Task<DataView> GetDataView(DataQuery dataQuery) => TransformNodes(await GetNodes(dataQuery));

    internal async Task<NodesList> GetNodes(DataQuery dataQuery)
    {
        var nodes = await GetData();
        var result = new NodesList(nodes, nodes.Count());
        
        if (dataQuery.Sort != null)
            foreach (var sortDefinition in dataQuery.Sort)
                result.Nodes = result.Nodes.Sort(sortDefinition.Key, sortDefinition.Value);

        if (dataQuery.Filter != null)
            foreach (var filterDefinition in dataQuery.Filter)
                result.Nodes = result.Nodes.Fitler(filterDefinition.Key, filterDefinition.Value);

        if (dataQuery.PageSize.GetValueOrDefault(-1) > -1)
            result.Nodes = result.Nodes.Take(dataQuery.PageSize!.Value);
        return result;
    }

    private DataView TransformNodes(NodesList nodes)
    {
        var rows = new List<Dictionary<DataColumn, object?>>();
        var result = new DataView(Columns.Values, rows, nodes.Count);
        foreach (var node in nodes.Nodes)
        {
            var row = new Dictionary<DataColumn, object?>();
            foreach (var column in Columns)
                try
                {
                    row[column.Value] = column.Value.ColumnDataType switch
                    {
                        DataType.Text => node[column.Key]?.GetValue<string>(),
                        DataType.Date => node[column.Key]?.GetValue<DateTime>(),
                        DataType.Precision => node[column.Key]?.GetValue<decimal>(),
                        DataType.Number => node[column.Key]?.GetValue<int>(),
                        _ => throw new Exception("Unsupported DataType")
                    };
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
}