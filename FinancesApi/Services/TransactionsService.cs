﻿using FinancesApi.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FinancesApi.Services
{
    public interface ITransactionsService
    {
        IList<Transaction> GetTransactions(string id = null);
        void SaveTransaction(Transaction transaction);
        void DeleteTransaction(string id);

        IList<TransactionAccount> GetAccounts();
        void SaveAccount(TransactionAccount account);
        void DeleteAccount(string id);

        IList<TransactionCategory> GetCategories();
        void SaveCategory(TransactionCategory category);
        void DeleteCategory(string id);
    }

    public class TransactionsService: ITransactionsService
    {
        private readonly Jsonfile<List<Transaction>> _transactions;
        private readonly Jsonfile<List<TransactionAccount>> _accounts;
        private readonly Jsonfile<List<TransactionCategory>> _categories;

        public TransactionsService(IConfiguration configuration)
        {
            var basePath = configuration.GetValue<string>("DatasetPath");
            var transactionsFile = Path.Combine(basePath, "transactions.json");
            _transactions = new Jsonfile<List<Transaction>>(transactionsFile);
            var accountsFile = Path.Combine(basePath, "transaction-accounts.json");
            _accounts = new Jsonfile<List<TransactionAccount>>(accountsFile);
            var categoriesFile = Path.Combine(basePath, "transaction-categories.json");
            _categories = new Jsonfile<List<TransactionCategory>>(categoriesFile);
        }

        public IList<TransactionAccount> GetAccounts()
        {
            _accounts.Load();
            return _accounts.Value.OrderBy(a => a.Title).ToList();
        }

        public void SaveAccount(TransactionAccount account)
        {
            _accounts.Load();
            var editedAccount = _accounts.Value.FirstOrDefault(a => string.Equals(account.Id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            if (editedAccount == null)
                _accounts.Value.Add(account);
            else
                editedAccount.Title = account.Title;
            _accounts.Save();
        }

        public void DeleteAccount(string id)
        {
            _accounts.Load();
            _accounts.Value.RemoveAll(a => string.Equals(id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            _accounts.Save();
        }

        public IList<Transaction> GetTransactions(string id = null)
        {
            _transactions.Load();
            //_transactions.Value.ForEach(t =>
            //{
            //    t.Category = t.Category[0].ToString().ToUpper() + t.Category.Substring(1).ToLower();
            //});
            //_transactions.Save();
            return string.IsNullOrWhiteSpace(id)
                ? _transactions.Value
                : _transactions.Value.Where(t => string.Equals(id, t.Id, System.StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        public void SaveTransaction(Transaction transaction)
        {
            _transactions.Load();
            var editedTransaction = _transactions.Value.FirstOrDefault(t => string.Equals(transaction.Id, t.Id, StringComparison.InvariantCultureIgnoreCase));
            if (editedTransaction == null)
                _transactions.Value.Add(transaction);
            else
                typeof(Transaction).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList().ForEach(prop => {
                    if (prop.CanWrite)
                        prop.SetValue(editedTransaction, prop.GetValue(transaction));
                });
            _transactions.Save();
        }

        public void DeleteTransaction(string id)
        {
            _transactions.Load();
            _transactions.Value.RemoveAll(a => string.Equals(id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            _transactions.Save();
        }

        public IList<TransactionCategory> GetCategories()
        {
            _categories.Load();
            //_transactions.Load();
            //var t = _transactions.Value.Where(_ => _.Date > DateTime.Now.AddDays(-14)).GroupBy(k => k.Category).Select(g=> new { Category = g.Key, Count = g.Count() }).ToList();
            //_categories.Value.ForEach(c =>
            //    {
            //        c.Title = c.Title[0].ToString().ToUpper() + c.Title.Substring(1).ToLower();
            //        c.UsageIndex = t.FirstOrDefault(tg => string.Equals(tg.Category, c.Title, StringComparison.InvariantCultureIgnoreCase))?.Count ?? 0;
            //    }
            //);
            //_categories.Value.ForEach(c => c.Id = Guid.NewGuid().ToString());
            //_categories.Save();
            return _categories.Value;
        }

        public void SaveCategory(TransactionCategory category)
        {
            _categories.Load();
            var edited = _categories.Value.FirstOrDefault(c => string.Equals(category.Id, c.Id, StringComparison.InvariantCultureIgnoreCase));
            if (edited == null)
                _categories.Value.Add(category);
            else
                edited.Title = category.Title;
            _categories.Save();
        }

        public void DeleteCategory(string id)
        {
            _categories.Load();
            _categories.Value.RemoveAll(c => string.Equals(id, c.Id, StringComparison.InvariantCultureIgnoreCase));
            _categories.Save();
        }
    }
}
