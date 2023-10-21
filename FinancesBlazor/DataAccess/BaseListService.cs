using FinancesBlazor.DataAccess;

namespace Finances.DataAccess
{
    public class BaseListService<T, TKey>
        where T: IDataIdentifier
    {
        private readonly IJsonListFile<T> _dataFile;
        private readonly Func<T, TKey> _defaultSorting;
        private readonly bool _ascending;

        public BaseListService(IJsonListFile<T> dataFile, Func<T, TKey> defaultSorting, bool ascending = true) { 
            _dataFile = dataFile;
            _defaultSorting = defaultSorting; 
            _ascending = ascending;
        }

        public virtual async Task<T[]> Get()
        {
            await _dataFile.Load();
            return _ascending 
                ? _dataFile.Data.OrderBy(_defaultSorting).ToArray() 
                : _dataFile.Data.OrderByDescending(_defaultSorting).ToArray();
        }

        public async Task Delete(string id)
        {
            await _dataFile.Load();
            _dataFile.Data.RemoveAll(a => string.Equals(id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            await _dataFile.Save();
        }
    }
}
