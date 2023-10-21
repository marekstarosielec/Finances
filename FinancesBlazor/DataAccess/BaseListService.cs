using FinancesBlazor.DataAccess;
using FinancesBlazor.Extensions;

namespace Finances.DataAccess
{
    public class BaseListService<T>
        where T: IDataIdentifier
    {
        private readonly IJsonListFile<T> _dataFile;

        public BaseListService(IJsonListFile<T> dataFile) { 
            _dataFile = dataFile;
        }

        public virtual async Task<T[]> Get(string? sortColumn, bool descending)
        {
            await _dataFile.Load();
            return sortColumn == null ? _dataFile.Data.ToArray() : _dataFile.Data.OrderByDynamic(sortColumn, descending).ToArray();
        }

        public virtual async Task Delete(string id)
        {
            await _dataFile.Load();
            _dataFile.Data.RemoveAll(a => string.Equals(id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            await _dataFile.Save();
        }

        public virtual async Task Save(T data)
        {
            await _dataFile.Load();
            _dataFile.Data.RemoveAll(a => string.Equals(data.Id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            _dataFile.Data.Add(data);
            await _dataFile.Save();
        }
    }
}
