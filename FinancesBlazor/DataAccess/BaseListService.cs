using FinancesBlazor.DataAccess;
using FinancesBlazor.Extensions;
using FinancesBlazor.ViewManager;
using System.Text.Json.Nodes;

namespace Finances.DataAccess;


public class BaseListService
{
    private readonly IJsonListFile _dataFile;
    public View View { get; set; }

    private static SemaphoreSlim semaphore = new(initialCount: 1);

    public List<JsonNode?> Data { get; private set; }

    public BaseListService(IJsonListFile dataFile)
    {
        _dataFile = dataFile;
    }

    public virtual async Task Reload()
    {
        await _dataFile.Load();
        Data = _dataFile.Data.GetDataForView(View);
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
