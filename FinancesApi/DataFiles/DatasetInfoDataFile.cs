using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FinancesApi.DataFiles
{
    public class DatasetInfoDataFile : Jsonfile<DatasetInfo>
    {
        public DatasetInfoDataFile(IConfiguration configuration) : base(configuration, "info.json")
        {
        }
    }
}
