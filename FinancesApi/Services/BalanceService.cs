using FinancesApi.DataFiles;
using FinancesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancesApi.Services
{
    public interface IBalanceService
    {
        IList<Balance> GetBalances(string id = null);
        void SaveBalance(Balance balance);
        void DeleteBalance(string id);

    }

    public class BalanceService: IBalanceService
    {
        private readonly BalancesDataFile _balancesDataFile;

        public BalanceService(BalancesDataFile balancesDataFile)
        {
            _balancesDataFile = balancesDataFile;
        }

        public IList<Balance> GetBalances(string id = null)
        {
            _balancesDataFile.Load();
            return string.IsNullOrWhiteSpace(id)
                ? _balancesDataFile.Value
                : _balancesDataFile.Value.Where(b => string.Equals(id, b.Id, System.StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        public void SaveBalance(Balance balance)
        {
            _balancesDataFile.Load();
            _balancesDataFile.Value.RemoveAll(b => b.Account == balance.Account && b.Date.Date == balance.Date.Date);
            _balancesDataFile.Value.Add(balance);
            _balancesDataFile.Save();
        }

        public void DeleteBalance(string id)
        {
            _balancesDataFile.Load();
            _balancesDataFile.Value.RemoveAll(b => string.Equals(id, b.Id, StringComparison.InvariantCultureIgnoreCase));
            _balancesDataFile.Save();
        }
    }
}
