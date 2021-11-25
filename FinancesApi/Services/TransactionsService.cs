using FinancesApi.Models;
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
        void SaveTransaction(Transaction transaction, bool overwriteEditableData = true);
        void DeleteTransaction(string id);

        IList<TransactionAccount> GetAccounts();
        void SaveAccount(TransactionAccount account);
        void DeleteAccount(string id);

        IList<TransactionCategory> GetCategories();
        void SaveCategory(TransactionCategory category);
        void DeleteCategory(string id);

        IList<TransactionAutoCategory> GetAutoCategories();
        void SaveAutoCategory(TransactionAutoCategory category);
        void DeleteAutoCategory(string id);
        
        void ApplyAutoCategories();
    }

    public class TransactionsService: ITransactionsService
    {
        private readonly Jsonfile<List<Transaction>> _transactions;
        private readonly Jsonfile<List<TransactionAccount>> _accounts;
        private readonly Jsonfile<List<TransactionCategory>> _categories;
        private readonly Jsonfile<List<TransactionAutoCategory>> _autoCategories;

        public TransactionsService(IConfiguration configuration)
        {
            var basePath = configuration.GetValue<string>("DatasetPath");
            var transactionsFile = Path.Combine(basePath, "transactions.json");
            _transactions = new Jsonfile<List<Transaction>>(transactionsFile);
            var accountsFile = Path.Combine(basePath, "transaction-accounts.json");
            _accounts = new Jsonfile<List<TransactionAccount>>(accountsFile);
            var categoriesFile = Path.Combine(basePath, "transaction-categories.json");
            _categories = new Jsonfile<List<TransactionCategory>>(categoriesFile);
            var autoCategoriesFile = Path.Combine(basePath, "transaction-auto-categories.json");
            _autoCategories = new Jsonfile<List<TransactionAutoCategory>>(autoCategoriesFile);
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
            //    if (t.Date.Hour == 23)
            //        t.Date = t.Date.AddHours(1);
            //    //t.Category = null;
            //});
            //_transactions.Save();
            return string.IsNullOrWhiteSpace(id)
                ? _transactions.Value
                : _transactions.Value.Where(t => string.Equals(id, t.Id, System.StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        public void SaveTransaction(Transaction transaction, bool overwriteEditableData=true)
        {
            _transactions.Load();
            var editedTransaction = _transactions.Value.FirstOrDefault(t => string.Equals(transaction.Id, t.Id, StringComparison.InvariantCultureIgnoreCase));
            if (editedTransaction == null)
                _transactions.Value.Add(transaction);
            else
            {
                editedTransaction.ScrappingDate = transaction.ScrappingDate;
                editedTransaction.Status = transaction.Status;
                editedTransaction.Source = transaction.Source;
                editedTransaction.Date = transaction.Date;
                editedTransaction.Account = transaction.Account;
                editedTransaction.Amount = transaction.Amount;
                editedTransaction.Title = transaction.Title;
                editedTransaction.Description = transaction.Description;
                editedTransaction.Text = transaction.Text;
                editedTransaction.Currency = transaction.Currency;
                if (overwriteEditableData)
                {
                    editedTransaction.Category = transaction.Category;
                    editedTransaction.Comment = transaction.Comment;
                    editedTransaction.Person = transaction.Person;
                    editedTransaction.Details = transaction.Details;
                }
            }
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

        public IList<TransactionAutoCategory> GetAutoCategories()
        {
            _autoCategories.Load();
           return _autoCategories.Value;
        }

        public void SaveAutoCategory(TransactionAutoCategory autoCategory)
        {
            _autoCategories.Load();
            var edited = _autoCategories.Value.FirstOrDefault(ac => string.Equals(autoCategory.Id, ac.Id, StringComparison.InvariantCultureIgnoreCase));
            if (edited == null)
                _autoCategories.Value.Add(autoCategory);
            else
            {
                edited.BankInfo = autoCategory.BankInfo;
                edited.Category = autoCategory.Category;
            }
            _autoCategories.Save();
        }

        public void DeleteAutoCategory(string id)
        {
            _autoCategories.Load();
            _autoCategories.Value.RemoveAll(c => string.Equals(id, c.Id, StringComparison.InvariantCultureIgnoreCase));
            _autoCategories.Save();
        }

        public void ApplyAutoCategories()
        {
            _autoCategories.Load();
            _transactions.Load();
            _transactions.Value.Where(t => string.IsNullOrWhiteSpace(t.Category)).ToList().ForEach(t => {
                var match = _autoCategories.Value.FirstOrDefault(ac => t.BankInfo.Contains(ac.BankInfo, StringComparison.InvariantCultureIgnoreCase));
                if (match != null)
                    t.Category = match.Category;
            });
            _transactions.Save();
        }

        //public void GetBills()
        //{
        //    _transactions.Load();
        //    _transactions.Value.GroupBy(key => key.Category + key.Date.ToString("yyyyMM"))
        //        .Select(g => g.Key, g)
        //}
    }
}
