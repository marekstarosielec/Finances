using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FinancesApi.DataFiles
{
    public class DocumentCategoryDataFile : Jsonfile<List<DocumentCategory>>
    {
        public DocumentCategoryDataFile(IConfiguration configuration) : base(configuration, "documentCategory.json")
        {
        }
    }
}
