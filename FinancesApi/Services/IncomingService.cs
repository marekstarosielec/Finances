using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace FinancesApi.Controllers
{
    public interface IIncomingService
    {
        IList<Incoming> Get();
    }

    public class IncomingService : IIncomingService
    {
        private readonly List<string> _incomingFolders;
        private readonly IDocumentService _documentService;

        public IncomingService(IConfiguration configuration, IDocumentService documentService)
        {
            _incomingFolders = configuration.GetSection("IncomingFolders").Get<List<string>>();
            _documentService = documentService;
        }

        public IList<Incoming> Get()
        {
            var result = new List<Incoming>();
            foreach (var folder in _incomingFolders)
                foreach (var file in Directory.EnumerateFiles(folder))
                    if (!File.GetAttributes(file).HasFlag(FileAttributes.Hidden))
                        result.Add(BuildModelForFile(file));
            return result;
        }

        private Incoming BuildModelForFile(string fileName) =>
            new()
            {
                FullFileName = fileName,
                FileName = Path.GetFileName(fileName),
                SortableFileName = GetSortableFileName(Path.GetFileName(fileName)).ToLower(),
                CreatedOn = File.GetCreationTime(fileName)
            };
         
        private string GetSortableFileName(string fileName)
        {
            if (string.Equals(fileName, "Obraz.jpg", StringComparison.InvariantCultureIgnoreCase))
                return "Obraz (001).jpg";
            else if (fileName.ToLowerInvariant().StartsWith("obraz ("))
                return "Obraz (" + fileName.Substring(7).PadLeft(8,'0');
            else
                return fileName;

        }
    }
}