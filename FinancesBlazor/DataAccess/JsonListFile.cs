using Finances.DataAccess;

namespace FinancesBlazor.DataAccess;

public interface IJsonListFile<T> : IJsonFile<List<T>> { }

public class JsonListFile<T> : JsonFile<List<T>>, IJsonListFile<T>
{
    public JsonListFile(IConfiguration configuration, string fileName) : base(configuration, fileName)
    {
    }
}
