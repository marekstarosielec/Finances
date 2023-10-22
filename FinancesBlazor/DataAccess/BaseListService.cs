using FinancesBlazor.DataAccess;
using System.Text.Json.Nodes;

namespace Finances.DataAccess;


public class BaseListService
{
    private readonly IJsonListFile _dataFile;
    private static SemaphoreSlim semaphore = new(initialCount: 1);

    public BaseListService(IJsonListFile dataFile)
    {
        _dataFile = dataFile;
    }

    public virtual async Task<JsonArray> Get()
    {
        await _dataFile.Load();
        // return sortColumn == null ? _dataFile.Data.ToArray() : _dataFile.Data.OrderByDynamic(sortColumn, descending).ToArray();
        return _dataFile.Data;
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
