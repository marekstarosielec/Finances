using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;

namespace FinancesApi.DataFiles
{
    public class DatasetInfoDataFile : Jsonfile<DatasetInfo>
    {
        public DatasetInfoDataFile(IConfiguration configuration) : base(configuration, "info.json")
        {
        }
    }
}
