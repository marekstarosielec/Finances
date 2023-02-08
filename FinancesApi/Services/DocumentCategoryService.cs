using FinancesApi.DataFiles;
using FinancesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancesApi.Controllers
{
    public interface IDocumentCategoryService
    {
        IList<DocumentCategory> GetDocumentCategory();
        void SaveDocumentCategory(DocumentCategory documentCategory);
        void DeleteDocumentCategory(string id);
    }

    public class DocumentCategoryService : IDocumentCategoryService
    {
        private readonly DocumentCategoryDataFile _documentCategoryDataFile;

        public DocumentCategoryService(DocumentCategoryDataFile documentCategoryDataFile)
        {
            _documentCategoryDataFile = documentCategoryDataFile;
        }

        public void DeleteDocumentCategory(string id)
        {
            _documentCategoryDataFile.Load();
            _documentCategoryDataFile.Value.RemoveAll(a => string.Equals(id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            _documentCategoryDataFile.Save();
        }

        public IList<DocumentCategory> GetDocumentCategory()
        {
            _documentCategoryDataFile.Load();
            return _documentCategoryDataFile.Value.OrderByDescending(a => a.Name).ToList();
        }

        public void SaveDocumentCategory(DocumentCategory documentCategory)
        {
            _documentCategoryDataFile.Load();
            var edited = _documentCategoryDataFile.Value.FirstOrDefault(a => string.Equals(documentCategory.Id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            if (edited == null)
            {
                if (string.IsNullOrWhiteSpace(documentCategory.Id))
                    documentCategory.Id = Guid.NewGuid().ToString();
                _documentCategoryDataFile.Value.Add(documentCategory);
            }
            else
            {
                edited.Name = documentCategory.Name;
            }
            _documentCategoryDataFile.Save();
        }
    }
}