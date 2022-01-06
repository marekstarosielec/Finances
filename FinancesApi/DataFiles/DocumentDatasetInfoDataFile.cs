using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;

namespace FinancesApi.DataFiles
{
    public class DocumentDatasetInfoDataFile : Jsonfile<DocumentDatasetInfo>
    {
        public DocumentDatasetInfoDataFile(IConfiguration configuration) : base(configuration, "documentInfo.json")
        {
        }
    }
}
