using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FinancesApi.DataFiles
{
    public class TransactionsDataFile : Jsonfile<List<Transaction>>
    {
        public TransactionsDataFile(IConfiguration configuration): base(configuration, "transactions.json")
        {
        }
    }
}
