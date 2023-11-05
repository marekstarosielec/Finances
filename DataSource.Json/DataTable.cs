using System.Text;
using System.Text.Json.Nodes;

namespace DataSource.Json;

public class DataTable : IDataSource
{
    private readonly string _fileName;
    protected static Dictionary<string, SemaphoreSlim> _semaphores = new ();
    private string? _contentString = null;
    public Dictionary<string, DataColumn> Columns { get; private set; }

    public DataTable(string fileName, params DataColumn[] dataColumns)
    {
        if (string.IsNullOrWhiteSpace(fileName)) 
            throw new ArgumentException("FileName cannot be whitespace");
        
        _fileName = fileName;
        Columns = dataColumns.ToDictionary(c => c.ColumnName, c => c);
    }

    public async Task<IEnumerable<JsonNode>> GetDataView(DataView view)
    {
        IEnumerable<JsonNode> result = await GetData();

        if (view.Sort != null)
            foreach (var sortDefinition in view.Sort)
                result = result.Sort(sortDefinition.Key, sortDefinition.Value);

        if (view.Filter != null)
            foreach (var filterDefinition in view.Filter)
                result = result.Fitler(filterDefinition.Key, filterDefinition.Value);

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