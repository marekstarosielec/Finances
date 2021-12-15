using FinancesApi.DataFiles;
using FinancesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancesApi.Services
{
    public interface IDocumentService
    {
        IList<Document> GetDocuments(string id = null);
        int GetMaxDocumentNumber();
        void SaveDocument(Document document);
        void DeleteDocument(string id);

    }

    public class DocumentService: IDocumentService
    {
        private readonly DocumentsDataFile _documentsDataFile;

        public DocumentService(DocumentsDataFile documentsDataFile)
        {
            _documentsDataFile = documentsDataFile;
        }

        public IList<Document> GetDocuments(string id = null)
        {
            _documentsDataFile.Load();
            return string.IsNullOrWhiteSpace(id)
                 ? _documentsDataFile.Value
                 : _documentsDataFile.Value.Where(d => string.Equals(id, d.Id, System.StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        public int GetMaxDocumentNumber()
        {
            _documentsDataFile.Load();
            return _documentsDataFile.Value.OrderByDescending(d => d.Number).First().Number;
        }

        public void SaveDocument(Document document)
        {
            _documentsDataFile.Load();
            var edited = _documentsDataFile.Value.FirstOrDefault(d => d.Number == document.Number);
            if (edited == null)
                _documentsDataFile.Value.Add(document);
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
            }
            _documentsDataFile.Save();
        }

        public void DeleteDocument(string id)
        {
            _documentsDataFile.Load();
            _documentsDataFile.Value.RemoveAll(b => string.Equals(id, b.Id, StringComparison.InvariantCultureIgnoreCase));
            _documentsDataFile.Save();
        }
    }
}
