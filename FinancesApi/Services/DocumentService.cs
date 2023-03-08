using FinancesApi.DataFiles;
using FinancesApi.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using Document = FinancesApi.Models.Document;

namespace FinancesApi.Services
{
    public interface IDocumentService
    {
        IList<Document> GetDocuments(string id = null);
        void SaveDocument(Document document, bool resetTransactionLink = true);
        void DeleteDocument(string id);
        string ConvertFileToDocument(string fileName);
        string ConvertFilesToPdf(List<string> fileNames);
    }

    public class DocumentService: IDocumentService
    {
        private readonly DocumentsDataFile _documentsDataFile;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;


        public DocumentService(DocumentsDataFile documentsDataFile, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _documentsDataFile = documentsDataFile;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public IList<Document> GetDocuments(string id = null)
        {
            UpdateNewFolders();
            _documentsDataFile.Load();
            return string.IsNullOrWhiteSpace(id)
                 ? _documentsDataFile.Value
                 : _documentsDataFile.Value.Where(d => string.Equals(id, d.Id, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private void UpdateNewFolders()
        {
            var merger = new Dictionary<string, Document>();
            var basePath = _configuration.GetValue<string>("DatasetPath");
            var documentsPath = Path.Combine(basePath, "Dokumenty");

            Dictionary<string, string> documentFiles = GetDocumentFiles(documentsPath);

            string number;
            _documentsDataFile.Load();
            _documentsDataFile.Value.ForEach(d =>
            {
                number = ConvertNumberToFileName(d.Number);
                merger[number] = d;
            });

            bool requiresUpdate = false;
            foreach (var document in documentFiles.Keys)
            {
                if (merger.ContainsKey(document))
                    continue;
                //Found file which is not in the list.
                merger[document] = new Document
                {
                    Id = Guid.NewGuid().ToString(),
                    Number = ConvertFileNameToNumber(document),
                    Date = DateTime.Now
                };
                requiresUpdate = true;
            };
            var result = merger.Values.ToList();
            if (requiresUpdate)
            {
                _documentsDataFile.Value.Clear();
                result.ForEach(r => _documentsDataFile.Value.Add(r));
                _documentsDataFile.Save();
            }
        }

        private Dictionary<string, string> GetDocumentFiles(string documentsPath)
        {
            var documents = new Dictionary<string, string>();
            Directory.EnumerateFileSystemEntries(documentsPath).ToList().ForEach(f =>
            {
                if (IsDocumentFile(f))
                    documents[Path.GetFileNameWithoutExtension(f)] = f;
            });
            return documents;
        }

        public string ConvertFileToDocument(string fileName)
        {
            _documentsDataFile.Load();
            var destinationFileName = ConvertNumberToFileName(_documentsDataFile.Value.OrderByDescending(d => d.Number).First().Number + 1);
            var basePath = _configuration.GetValue<string>("DatasetPath");
            var destinationFolder = Path.Combine(basePath, "Dokumenty", destinationFileName);
            var fullDestinationName = Path.Combine(destinationFolder, Path.GetFileName(fileName));
            Directory.CreateDirectory(destinationFolder);
            File.Move(fileName, fullDestinationName);
            SetSecurity(fullDestinationName);
            UpdateNewFolders();
            _documentsDataFile.Load();
            var id = _documentsDataFile.Value.Last().Id;
            return id;
        }

        public string ConvertFilesToPdf(List<string> fileNames)
        {
            _documentsDataFile.Load();
            var destinationFileName = ConvertNumberToFileName(_documentsDataFile.Value.OrderByDescending(d => d.Number).First().Number + 1);
            var basePath = _configuration.GetValue<string>("DatasetPath");
            var destinationFolder = Path.Combine(basePath, "Dokumenty", destinationFileName);
            var fullDestinationName = Path.Combine(destinationFolder, "Skan.pdf");
            Directory.CreateDirectory(destinationFolder);

            byte[] pdfBytes;
            using (MemoryStream ms = new MemoryStream())
                using (var doc = new iTextSharp.text.Document(PageSize.LETTER, 1.0f, 1.0f, 1.0f, 1.0f))
                    using (PdfWriter writer = PdfWriter.GetInstance(doc, ms))
                    {
                        doc.Open();
                        foreach(var fileName in fileNames)
                        {
                            doc.NewPage();
                            var img = Image.GetInstance(fileName);
                            PdfPTable table = new PdfPTable(1);
                            table.AddCell(new PdfPCell(img));
                            doc.Add(table);
                        }
                        doc.Close();
                        pdfBytes = ms.ToArray();
                        File.WriteAllBytes(fullDestinationName, pdfBytes);
                    }


            UpdateNewFolders();
            _documentsDataFile.Load();
            var id = _documentsDataFile.Value.Last().Id;
            return id;
        }

        public static void SetSecurity(string fileName)
        {
            var path = Path.GetFullPath(fileName);
            var di = new DirectoryInfo(path);
            DirectorySecurity sec = di.GetAccessControl();
            // Using this instead of the "Everyone" string means we work on non-English systems.
            SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            sec.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.Modify | FileSystemRights.Synchronize, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
            di.SetAccessControl(sec);
        }

        public void SaveDocument(Document document, bool resetTransactionLink = true)
        {
            var transactionService = (ITransactionsService) _serviceProvider.GetService(typeof(ITransactionsService));
            _documentsDataFile.Load();
            var edited = _documentsDataFile.Value.FirstOrDefault(d => d.Number == document.Number);

            if (edited == null)
            {
                if (string.IsNullOrWhiteSpace(document.Id))
                    document.Id = Guid.NewGuid().ToString();
                _documentsDataFile.Value.Add(document);
            }
            else
            {
                edited.Date = document.Date;
                edited.Number = document.Number;
                edited.Pages = document.Pages;
                edited.Description = document.Description;
                edited.Category = document.Category;
                edited.InvoiceNumber = document.InvoiceNumber;
                edited.Company = document.Company;
                edited.Person = document.Person;
                edited.Car = document.Car;
                edited.RelatedObject = document.RelatedObject;
                edited.Guarantee = document.Guarantee;
                edited.CaseName = document.CaseName;
                edited.Settlement = document.Settlement;
                edited.Net = document.Net;
                edited.Vat = document.Vat;
                edited.Gross = document.Gross;
                edited.Currency = document.Currency;

                if (resetTransactionLink && edited.TransactionId != document.TransactionId)
                {
                    if (!string.IsNullOrWhiteSpace(edited.TransactionId))
                    {
                        //Transaction changed. Detouch previous one.
                        var previousTransaction = transactionService.GetTransactions(edited.TransactionId).FirstOrDefault();
                        if (previousTransaction != null)
                        {
                            previousTransaction.DocumentId = null;
                            previousTransaction.DocumentCategory = null;
                            previousTransaction.DocumentInvoiceNumber = null;
                            previousTransaction.DocumentNumber = null;
                            transactionService.SaveTransaction(previousTransaction, resetDocumentLink: false);
                        }
                    }

                    //New transaction attached.
                    if (!string.IsNullOrWhiteSpace(document.TransactionId))
                    { 
                        var newTransaction = transactionService.GetTransactions(document.TransactionId).FirstOrDefault();
                        newTransaction.DocumentId = document.Id;
                        newTransaction.DocumentCategory = document.Category;
                        newTransaction.DocumentInvoiceNumber = document.InvoiceNumber;
                        newTransaction.DocumentNumber = document.Number;
                        if (!string.IsNullOrWhiteSpace(document.Settlement))
                            newTransaction.Settlement = document.Settlement;
                        else
                            document.Settlement = newTransaction.Settlement;
                        transactionService.SaveTransaction(newTransaction, resetDocumentLink: false);
                    }  

                    //Save transaction information in document
                    if (!string.IsNullOrWhiteSpace(document.TransactionId))
                    {
                        var relatedTransaction = transactionService.GetTransactions(document.TransactionId).FirstOrDefault();
                        edited.TransactionCategory = relatedTransaction?.Category;
                        edited.TransactionAmount = relatedTransaction?.Amount;
                        edited.TransactionCurrency = relatedTransaction?.Currency;
                        edited.TransactionBankInfo = relatedTransaction?.BankInfo;
                        edited.TransactionComment = relatedTransaction?.Comment;
                    } else
                    {
                        edited.TransactionCategory = null;
                        edited.TransactionAmount = null;
                        edited.TransactionCurrency = null;
                        edited.TransactionBankInfo = null;
                        edited.TransactionComment = null;
                    }
                }
                if (!resetTransactionLink)
                {
                    edited.TransactionCategory = document.TransactionCategory;
                    edited.TransactionAmount = document.TransactionAmount;
                    edited.TransactionCurrency = document.TransactionCurrency;
                    edited.TransactionBankInfo = document.TransactionBankInfo;
                    edited.TransactionComment = document.TransactionComment;
                }
                edited.TransactionId = document.TransactionId;

            }
            _documentsDataFile.Save();
        }

        public void DeleteDocument(string id)
        {
            _documentsDataFile.Load();
            _documentsDataFile.Value.RemoveAll(b => string.Equals(id, b.Id, StringComparison.InvariantCultureIgnoreCase));
            _documentsDataFile.Save();
        }

        private string ConvertNumberToFileName(int number) => "MX" + number.ToString().PadLeft(5, '0');
        private int ConvertFileNameToNumber(string input)
        {
            if (!int.TryParse(input.Substring(2), NumberStyles.None, null, out var result))
                return 0;
            return result;
        }

        private bool IsDocumentFile(string fileName)
        {
            var f = Path.GetFileNameWithoutExtension(fileName);
            if (f.Length != 7)
                return false;

            if (!f.ToUpper().StartsWith("MX"))
                return false;

            if (!int.TryParse(f.Substring(2), NumberStyles.None, null, out var result))
                return false;

            return true;
        }
    }
}
