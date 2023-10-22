using FinancesBlazor.DataAccess;
using FinancesBlazor.Extensions;
using FinancesBlazor.ViewManager;
using System.Text.Json.Nodes;

namespace Finances.DataAccess;


public class BaseListService
{
    private readonly IJsonListFile _dataFile;
    private readonly ViewListParameters _parameters;
    private static SemaphoreSlim semaphore = new(initialCount: 1);

    public BaseListService(IJsonListFile dataFile, ViewListParameters parameters)
    {
        _dataFile = dataFile;
        _parameters = parameters;
    }

    public virtual async Task<List<JsonNode?>> Get()
    {
        await _dataFile.Load();
        return _dataFile.Data.FilterByParameters(_parameters);
    }

    //public virtual async Task Delete(string id)
    //{
    //    try
    //    {
    //        semaphore.Wait();

    //        await _dataFile.Load();
    //        _dataFile.Data.RemoveAll(a => string.Equals(id, a.Id, StringComparison.InvariantCultureIgnoreCase));
    //        await _dataFile.Save();
    //    }
    //    finally
    //    {
    //        semaphore.Release();
    //    }
    //}

    //public virtual async Task Save(T data)
    //{
    //    try
    //    {
    //        semaphore.Wait();

    //        await _dataFile.Load();
    //        _dataFile.Data.RemoveAll(a => string.Equals(data.Id, a.Id, StringComparison.InvariantCultureIgnoreCase));
    //        _dataFile.Data.Add(data);
    //        await _dataFile.Save();
    //    }
    //    finally
    //    {
    //        semaphore.Release();
    //    }
    //}
}
