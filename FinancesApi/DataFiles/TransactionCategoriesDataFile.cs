using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FinancesApi.DataFiles
{
    public class TransactionCategoriesDataFile : Jsonfile<List<TransactionCategory>>
    {
        public TransactionCategoriesDataFile(IConfiguration configuration) : base(configuration, "transaction-categories.json")
        {
        }
    }
}
