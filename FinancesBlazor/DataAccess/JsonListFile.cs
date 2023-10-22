using Finances.DataAccess;
using System.Threading;

namespace FinancesBlazor.DataAccess;

//public interface IJsonListFile<T> : IJsonFile<List<T>> { }

//public class JsonListFile<T> : JsonFile<List<T>>, IJsonListFile<T>
//{
//    public JsonListFile(IConfiguration configuration, string fileName) : base(configuration, fileName)
//    {
//    }
//}


public interface IJsonListFile : IJsonFile { }

public class JsonListFile : JsonFile, IJsonListFile
{
    public JsonListFile(IConfiguration configuration, string fileName) : base(configuration, fileName)
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
