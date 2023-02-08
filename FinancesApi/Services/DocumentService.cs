using FinancesApi.DataFiles;
using FinancesApi.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        private readonly IConfiguration _configuration;

        public DocumentService(DocumentsDataFile documentsDataFile, IConfiguration configuration)
        {
            _documentsDataFile = documentsDataFile;
            _configuration = configuration;
        }

        public IList<Document> GetDocuments(string id = null)
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
            return string.IsNullOrWhiteSpace(id)
                 ? result
                 : result.Where(d => string.Equals(id, d.Id, StringComparison.InvariantCultureIgnoreCase)).ToList();
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

        public void SaveDocument(Document document)
        {
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
