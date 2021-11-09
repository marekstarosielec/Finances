using FinancesApi.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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
        private readonly Jsonfile<List<Balance>> _balances;

        public BalanceService(IConfiguration configuration)
        {
            var basePath = configuration.GetValue<string>("DatasetPath");
            var balancesFile = Path.Combine(basePath, "balances.json");
            _balances = new Jsonfile<List<Balance>>(balancesFile);
        }

        public IList<Balance> GetBalances(string id = null)
        {
            _balances.Load();
            return string.IsNullOrWhiteSpace(id)
                ? _balances.Value
                : _balances.Value.Where(b => string.Equals(id, b.Id, System.StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        public void SaveBalance(Balance balance)
        {
            _balances.Load();
            _balances.Value.RemoveAll(b => b.Account == balance.Account && b.Date.Date == balance.Date.Date);
            _balances.Value.Add(balance);
            _balances.Save();
        }

        public void DeleteBalance(string id)
        {
            _balances.Load();
            _balances.Value.RemoveAll(b => string.Equals(id, b.Id, StringComparison.InvariantCultureIgnoreCase));
            _balances.Save();
        }
    }
}
