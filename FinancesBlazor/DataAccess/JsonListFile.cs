using Finances.DataAccess;

namespace FinancesBlazor.DataAccess;

public interface IJsonListFile : IJsonFile { }

public class JsonListFile : JsonFile, IJsonListFile
{
    public JsonListFile(IConfiguration configuration, string fileName, JoinDefinition? join = null) : base(configuration, fileName, join)
    {
    }

    protected override async Task CreateEmptyDataFile()
    {
        try
        {
            semaphore.Wait();

            await File.WriteAllTextAsync(DataFile.FileNameWithLocation, "[]");
        }
        finally
        {
            semaphore.Release();
        }
    }
}
