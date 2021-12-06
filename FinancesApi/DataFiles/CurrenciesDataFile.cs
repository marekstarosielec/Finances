using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FinancesApi.DataFiles
{
    public class CurrenciesDataFile : Jsonfile<List<Currency>>
    {
        public CurrenciesDataFile(IConfiguration configuration) : base(configuration, "currencies.json")
        {
        }
    }
}
