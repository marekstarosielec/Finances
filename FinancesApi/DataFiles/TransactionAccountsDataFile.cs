using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FinancesApi.DataFiles
{
    public class TransactionAccountsDataFile : Jsonfile<List<TransactionAccount>>
    {
        public TransactionAccountsDataFile(IConfiguration configuration) : base(configuration, "transaction-accounts.json")
        {
        }
    }
}
