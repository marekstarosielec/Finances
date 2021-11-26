using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FinancesApi.DataFiles
{
    public class TransactionAutoCategoriesDataFile : Jsonfile<List<TransactionAutoCategory>>
    {
        public TransactionAutoCategoriesDataFile(IConfiguration configuration) : base(configuration, "transaction-auto-categories.json")
        {
        }
    }
}
