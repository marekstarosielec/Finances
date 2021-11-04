using FinancesApi.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FinancesApi.Services
{
    public interface ITransactionsService
    {
        IList<Transaction> GetTransactions(string id = null);
        IList<TransactionAccount> GetAccounts();
        void SaveAccount(TransactionAccount account);
    }

    public class TransactionsService: ITransactionsService
    {
        private readonly Jsonfile<IList<Transaction>> _transactions;
        private readonly Jsonfile<IList<TransactionAccount>> _accounts;

        public TransactionsService(IConfiguration configuration)
        {
            var basePath = configuration.GetValue<string>("DatasetPath");
            var transactionsFile = Path.Combine(basePath, "transactions.json");
            _transactions = new Jsonfile<IList<Transaction>>(transactionsFile);
            var accountsFile = Path.Combine(basePath, "transaction-accounts.json");
            _accounts = new Jsonfile<IList<TransactionAccount>>(accountsFile);
        }

        public IList<TransactionAccount> GetAccounts()
        {
            _accounts.Load();
            return _accounts.Value;
        }

        public void SaveAccount(TransactionAccount account)
        {
            _accounts.Load();
            var editedAccount = _accounts.Value.FirstOrDefault(a => string.Equals(account.Id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            if (editedAccount == null)
                return;
            editedAccount.Title = account.Title;
            _accounts.Save();
        }

        public IList<Transaction> GetTransactions(string id = null)
        {
            _transactions.Load();
            return string.IsNullOrWhiteSpace(id)
                ? _transactions.Value
                : _transactions.Value.Where(t => string.Equals(id, t.ScrapID, System.StringComparison.InvariantCultureIgnoreCase)).ToList();
        }
    }
}
