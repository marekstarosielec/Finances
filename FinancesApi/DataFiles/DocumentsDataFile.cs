using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FinancesApi.DataFiles
{
    public class DocumentsDataFile : Jsonfile<List<Document>>
    {
        public DocumentsDataFile(IConfiguration configuration): base(configuration, "documents.json")
        {
        }
    }
}
