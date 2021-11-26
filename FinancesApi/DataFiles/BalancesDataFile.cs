using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FinancesApi.DataFiles
{
    public class BalancesDataFile : Jsonfile<List<Balance>>
    {
        public BalancesDataFile(IConfiguration configuration): base(configuration, "balances.json")
        {
        }
    }
}
