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
            for (var x = _balancesDataFile.Value.Count - 1; x > 0; x--)
            {
                if (_balancesDataFile.Value[x].Account=="VISA PAYWAVE" || _balancesDataFile.Value[x].Account == "MASTERCARD PAYPASS")
                {
                    _balancesDataFile.Value.RemoveAt(x);
                }
            }
            _balancesDataFile.Save();
            return string.IsNullOrWhiteSpace(id)
                ? _balancesDataFile.Value
                : _balancesDataFile.Value.Where(b => string.Equals(id, b.Id, System.StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        public void SaveBalance(Balance balance)
        {
            _balancesDataFile.Load();
            _balancesDataFile.Value.RemoveAll(b => b.Account == balance.Account && b.Date.Date == balance.Date.Date);
            var edited = _balancesDataFile.Value.FirstOrDefault(a => string.Equals(balance.Id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            if (edited == null)
            {
                if (string.IsNullOrWhiteSpace(balance.Id))
                    balance.Id = Guid.NewGuid().ToString();
                _balancesDataFile.Value.Add(balance);
            }
            else
            {
                edited.Date = balance.Date;
                edited.Account = balance.Account;
                edited.Amount = balance.Amount;
                edited.Currency = balance.Currency;
            }
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
