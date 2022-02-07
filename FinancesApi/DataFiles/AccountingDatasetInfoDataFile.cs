using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;

namespace FinancesApi.DataFiles
{
    public class AccountingDatasetInfoDataFile : Jsonfile<AccountingDatasetInfo>
    {
        public AccountingDatasetInfoDataFile(IConfiguration configuration) : base(configuration, "accountingInfo.json")
        {
        }
    }
}
