using FinancesBlazor.DataAccess;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Finances.DataAccess;

public interface IJsonFile
{
    Task Load();
    Task Save();
    JsonArray Data { get; }
}


public abstract class JsonFile : IJsonFile
{
    protected static SemaphoreSlim semaphore = new(initialCount: 1);
    private readonly JoinDefinition? _join;

    public DataFile DataFile { get; }
    public DataFile? JoinedDataFile { get; }

    public JsonArray Data { get; private set; } = default!;

    public JsonFile(IConfiguration configuration, string fileName, JoinDefinition? join = null)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException($"'{nameof(fileName)}' cannot be null or whitespace.", nameof(fileName));

        var basePath = configuration.GetValue<string>("DatasetPath");
        DataFile = new DataFile(fileName, basePath);
        if (join != null)
        {
            JoinedDataFile = new DataFile(join.FileName, basePath);
            _join = join;
        }
    }

    public async Task Load()
    {
        try
        {
            semaphore.Wait();

            if (string.IsNullOrWhiteSpace(DataFile.FileName))
                throw new InvalidOperationException();

            if (!File.Exists(DataFile.FileNameWithLocation))
                await CreateEmptyDataFile();

            string jsonString = await File.ReadAllTextAsync(DataFile.FileNameWithLocation, Encoding.Latin1);
            Data = JsonNode.Parse(jsonString)?.AsArray() ?? throw new InvalidOperationException("Failed to deserialize data");
            
            if (JoinedDataFile != null)
            {
                string joinedJsonString = await File.ReadAllTextAsync(JoinedDataFile.FileNameWithLocation, Encoding.Latin1);
                var JoinedData = JsonNode.Parse(joinedJsonString)?.AsArray() ?? throw new InvalidOperationException("Failed to deserialize data");
                if (JoinedData != null)
                {
                    for (var x = 0; x < Data.Count; x++)
                    {
                        var row = Data[x];
                        if (row == null)
                            continue;
                        var joinId = row[_join!.JoinColumn]?.GetValue<string>();
                        if (joinId != null)
                        {
                            var relatedRecord = JoinedData.FirstOrDefault(j => j?["Id"]?.GetValue<string>() == joinId);
                            row[_join.JoinParentNodeName] = relatedRecord.Deserialize<JsonNode>();
                        }
                    }
                }
            }
        }

        finally
        {
            semaphore.Release();
        }

    }

    public async Task Save()
    {
        try
        {
            semaphore.Wait();

            var serializedValue = JsonSerializer.Serialize(Data.ToJsonString());
            await File.WriteAllTextAsync(DataFile.FileNameWithLocation, serializedValue);
        }
        finally
        {
            semaphore.Release();
        }
    }

    protected virtual async Task CreateEmptyDataFile()
    {
        try
        {
            semaphore.Wait();

            await File.WriteAllTextAsync(DataFile.FileNameWithLocation, "{}");
        }
        finally
        {
            semaphore.Release();
        }
    }
}