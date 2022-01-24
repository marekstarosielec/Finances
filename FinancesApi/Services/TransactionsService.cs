using FinancesApi.DataFiles;
using FinancesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly TransactionsDataFile _transactionsFile;
        private readonly TransactionAccountsDataFile _transactionAccountsDataFile;
        private readonly TransactionCategoriesDataFile _transactionCategoriesDataFile;
        private readonly TransactionAutoCategoriesDataFile _transactionAutoCategoriesDataFile;

        public TransactionsService( 
            TransactionsDataFile transactionsFile,
            TransactionAccountsDataFile transactionAccountsDataFile,
            TransactionCategoriesDataFile transactionCategoriesDataFile,
            TransactionAutoCategoriesDataFile transactionAutoCategoriesDataFile)
        {
            _transactionsFile = transactionsFile;
            _transactionAccountsDataFile = transactionAccountsDataFile;
            _transactionCategoriesDataFile = transactionCategoriesDataFile;
            _transactionAutoCategoriesDataFile = transactionAutoCategoriesDataFile;
        }

        public IList<TransactionAccount> GetAccounts()
        {
            _transactionAccountsDataFile.Load();
            //_transactionAccountsDataFile.Value.ForEach(a =>
            //{
            //    a.Currency = "PLN";
            //});
            //_transactionAccountsDataFile.Save();
            return _transactionAccountsDataFile.Value.OrderBy(a => a.Title).ToList();
        }

        public void SaveAccount(TransactionAccount account)
        {
            _transactionAccountsDataFile.Load();
            var editedAccount = _transactionAccountsDataFile.Value.FirstOrDefault(a => string.Equals(account.Id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            if (editedAccount == null)
                _transactionAccountsDataFile.Value.Add(account);
            else
                editedAccount.Title = account.Title;
            _transactionAccountsDataFile.Save();
        }

        public void DeleteAccount(string id)
        {
            _transactionAccountsDataFile.Load();
            _transactionAccountsDataFile.Value.RemoveAll(a => string.Equals(id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            _transactionAccountsDataFile.Save();
        }

        public IList<Transaction> GetTransactions(string id = null)
        {
            //_transactionCategoriesDataFile.Load();
            //_transactionsFile.Value.ForEach(t =>
            //{
            //    if (_transactionCategoriesDataFile.Value.FirstOrDefault(c => c.Title == t.Category) == null)
            //    {
            //        var match = _transactionCategoriesDataFile.Value.FirstOrDefault(c => string.Equals(c.Title, t.Category, StringComparison.InvariantCultureIgnoreCase));
            //        if (match != null)
            //            t.Category = match.Title;
            //        else if (string.Equals(t.Category, "mazda inne", StringComparison.InvariantCultureIgnoreCase))
            //            t.Category = "Mazda eksploatacja";
            //        else if (string.Equals(t.Category, "kot", StringComparison.InvariantCultureIgnoreCase))
            //            t.Category = "Koty";
            //        else if (t.Category == null) { }
            //        else
            //        {

            //        }
            //    }
            //});
            //_transactionsFile.Save();
            return string.IsNullOrWhiteSpace(id)
                ? _transactionsFile.Value
                : _transactionsFile.Value.Where(t => string.Equals(id, t.Id, System.StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        public void SaveTransaction(Transaction transaction, bool overwriteEditableData=true)
        {
            _transactionsFile.Load();
            var editedTransaction = _transactionsFile.Value.FirstOrDefault(t => string.Equals(transaction.Id, t.Id, StringComparison.InvariantCultureIgnoreCase));
            if (editedTransaction == null)
                _transactionsFile.Value.Add(transaction);
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
            _transactionsFile.Save();
        }

        public void DeleteTransaction(string id)
        {
            _transactionsFile.Load();
            _transactionsFile.Value.RemoveAll(a => string.Equals(id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            _transactionsFile.Save();
        }

        public IList<TransactionCategory> GetCategories()
        {
            _transactionCategoriesDataFile.Load();
            //_transactions.Load();
            //var t = _transactions.Value.Where(_ => _.Date > DateTime.Now.AddDays(-14)).GroupBy(k => k.Category).Select(g=> new { Category = g.Key, Count = g.Count() }).ToList();
            //_categories.Value.ForEach(c =>
            //    {
            //        c.Title = c.Title[0].ToString().ToUpper() + c.Title.Substring(1).ToLower();
            //        c.UsageIndex   = t.FirstOrDefault(tg => string.Equals(tg.Category, c.Title, StringComparison.InvariantCultureIgnoreCase))?.Count ?? 0;
            //    }
            //);
            //_categories.Value.ForEach(c => c.Id = Guid.NewGuid().ToString());
            //_categories.Save();
            return _transactionCategoriesDataFile.Value;
        }

        public void SaveCategory(TransactionCategory category)
        {
            _transactionCategoriesDataFile.Load();
            var edited = _transactionCategoriesDataFile.Value.FirstOrDefault(c => string.Equals(category.Id, c.Id, StringComparison.InvariantCultureIgnoreCase));
            if (edited == null)
                _transactionCategoriesDataFile.Value.Add(category);
            else
            {
                edited.Title = category.Title;
                edited.Deleted = category.Deleted;
            }
            _transactionCategoriesDataFile.Save();
        }

        public void DeleteCategory(string id)
        {
            _transactionCategoriesDataFile.Load();
            _transactionCategoriesDataFile.Value.RemoveAll(c => string.Equals(id, c.Id, StringComparison.InvariantCultureIgnoreCase));
            _transactionCategoriesDataFile.Save();
        }

        public IList<TransactionAutoCategory> GetAutoCategories()
        {
            _transactionAutoCategoriesDataFile.Load();
           return _transactionAutoCategoriesDataFile.Value;
        }

        public void SaveAutoCategory(TransactionAutoCategory autoCategory)
        {
            _transactionAutoCategoriesDataFile.Load();
            var edited = _transactionAutoCategoriesDataFile.Value.FirstOrDefault(ac => string.Equals(autoCategory.Id, ac.Id, StringComparison.InvariantCultureIgnoreCase));
            if (edited == null)
                _transactionAutoCategoriesDataFile.Value.Add(autoCategory);
            else
            {
                edited.BankInfo = autoCategory.BankInfo;
                edited.Category = autoCategory.Category;
            }
            _transactionAutoCategoriesDataFile.Save();
        }

        public void DeleteAutoCategory(string id)
        {
            _transactionAutoCategoriesDataFile.Load();
            _transactionAutoCategoriesDataFile.Value.RemoveAll(c => string.Equals(id, c.Id, StringComparison.InvariantCultureIgnoreCase));
            _transactionAutoCategoriesDataFile.Save();
        }

        public void ApplyAutoCategories()
        {
            _transactionAutoCategoriesDataFile.Load();
            _transactionsFile.Load();
            _transactionsFile.Value.Where(t => string.IsNullOrWhiteSpace(t.Category)).ToList().ForEach(t => {
                var match = _transactionAutoCategoriesDataFile.Value.FirstOrDefault(ac => t.BankInfo.Contains(ac.BankInfo, StringComparison.InvariantCultureIgnoreCase));
                if (match != null)
                    t.Category = match.Category;
            });
            _transactionsFile.Save();
        }
    }
}
