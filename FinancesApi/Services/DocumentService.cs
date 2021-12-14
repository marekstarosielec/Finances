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
