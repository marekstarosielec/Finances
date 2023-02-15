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
        void SaveTransaction(Transaction transaction, bool overwriteEditableData = true, bool resetDocumentLink = true);
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

        string GetAutoCategory(Transaction transaction);
        void ApplyAutoCategories();
    }

    public class TransactionsService: ITransactionsService
    {
        private readonly TransactionsDataFile _transactionsFile;
        private readonly TransactionAccountsDataFile _transactionAccountsDataFile;
        private readonly TransactionCategoriesDataFile _transactionCategoriesDataFile;
        private readonly TransactionAutoCategoriesDataFile _transactionAutoCategoriesDataFile;
        private readonly IDocumentService _documentService;

        public TransactionsService( 
            TransactionsDataFile transactionsFile,
            TransactionAccountsDataFile transactionAccountsDataFile,
            TransactionCategoriesDataFile transactionCategoriesDataFile,
            TransactionAutoCategoriesDataFile transactionAutoCategoriesDataFile,
            IDocumentService documentService)
        {
            _transactionsFile = transactionsFile;
            _transactionAccountsDataFile = transactionAccountsDataFile;
            _transactionCategoriesDataFile = transactionCategoriesDataFile;
            _transactionAutoCategoriesDataFile = transactionAutoCategoriesDataFile;
            _documentService = documentService;
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
            {
                if (string.IsNullOrWhiteSpace(account.Id))
                    account.Id = Guid.NewGuid().ToString();
                _transactionAccountsDataFile.Value.Add(account);
            }
            else
            {
                editedAccount.Title = account.Title;
                editedAccount.Deleted = account.Deleted;
            }
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
            _transactionsFile.Load();
            return string.IsNullOrWhiteSpace(id)
                ? _transactionsFile.Value.OrderByDescending(t => t.Date).ToList()
                : _transactionsFile.Value.Where(t => string.Equals(id, t.Id, System.StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        public void SaveTransaction(Transaction transaction, bool overwriteEditableData=true, bool resetDocumentLink = true)
        {
            _transactionsFile.Load();
            var editedTransaction = _transactionsFile.Value.FirstOrDefault(t => string.Equals(transaction.Id, t.Id, StringComparison.InvariantCultureIgnoreCase));
            
            if (editedTransaction == null)
            {
                if (string.IsNullOrWhiteSpace(transaction.Id))
                    transaction.Id = Guid.NewGuid().ToString();  
                _transactionsFile.Value.Add(transaction);
            }
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
                    editedTransaction.CaseName = transaction.CaseName;
                    editedTransaction.Settlement = transaction.Settlement;
                    
                    if (resetDocumentLink && editedTransaction.DocumentId != transaction.DocumentId)
                    {
                        if (!string.IsNullOrWhiteSpace(editedTransaction.DocumentId))
                        {
                            //Document changed. Detouch previous one.
                            var previousDocument = _documentService.GetDocuments(editedTransaction.DocumentId).FirstOrDefault();
                            if (previousDocument != null)
                            {
                                previousDocument.TransactionId = null;
                                previousDocument.TransactionCategory = null;
                                previousDocument.TransactionAmount = null;
                                previousDocument.TransactionCurrency = null;
                                previousDocument.TransactionBankInfo = null;
                                previousDocument.TransactionComment = null;
                                _documentService.SaveDocument(previousDocument, false);
                            }
                        }

                        //New document attached.
                        if (!string.IsNullOrWhiteSpace(transaction.DocumentId))
                        {
                            var newDocument = _documentService.GetDocuments(transaction.DocumentId).FirstOrDefault();
                            newDocument.TransactionId = transaction.Id;
                            newDocument.TransactionCategory = transaction.Category;
                            newDocument.TransactionAmount = transaction.Amount;
                            newDocument.TransactionCurrency = transaction.Currency;
                            newDocument.TransactionBankInfo = transaction.BankInfo;
                            newDocument.TransactionComment = transaction.Comment;
                            _documentService.SaveDocument(newDocument, false);
                        }

                        //Save document information in transaction
                        if (!string.IsNullOrWhiteSpace(transaction.DocumentId))
                        {
                            var relatedDocument = _documentService.GetDocuments(transaction.DocumentId).FirstOrDefault();
                            editedTransaction.DocumentCategory = relatedDocument?.Category;
                            editedTransaction.DocumentInvoiceNumber = relatedDocument?.InvoiceNumber;
                            editedTransaction.DocumentNumber = relatedDocument?.Number;
                        }else
                        {
                            editedTransaction.DocumentCategory = null;
                            editedTransaction.DocumentInvoiceNumber = null;
                            editedTransaction.DocumentNumber = null;
                        }
                    }
                    if (!resetDocumentLink)
                    {
                        editedTransaction.DocumentCategory = transaction.DocumentCategory;
                        editedTransaction.DocumentInvoiceNumber = transaction.DocumentInvoiceNumber;
                        editedTransaction.DocumentNumber = transaction.DocumentNumber;
                    }
                    editedTransaction.DocumentId = transaction.DocumentId;

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
            return _transactionCategoriesDataFile.Value;
        }

        public void SaveCategory(TransactionCategory category)
        {
            _transactionCategoriesDataFile.Load();
            var edited = _transactionCategoriesDataFile.Value.FirstOrDefault(c => string.Equals(category.Id, c.Id, StringComparison.InvariantCultureIgnoreCase));
            if (edited == null)
            {
                if (string.IsNullOrWhiteSpace(category.Id))
                    category.Id = Guid.NewGuid().ToString();
                _transactionCategoriesDataFile.Value.Add(category);
            }
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
            {
                if (string.IsNullOrWhiteSpace(autoCategory.Id))
                    autoCategory.Id = Guid.NewGuid().ToString();
                _transactionAutoCategoriesDataFile.Value.Add(autoCategory);
            }
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

        public string GetAutoCategory(Transaction transaction)
        {
            _transactionAutoCategoriesDataFile.Load();
            var autoCategory = _transactionAutoCategoriesDataFile.Value.FirstOrDefault(ac => transaction.BankInfo.Contains(ac.BankInfo, StringComparison.InvariantCultureIgnoreCase));
            return autoCategory?.Category;
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
