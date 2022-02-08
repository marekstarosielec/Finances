using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FinancesApi.DataFiles
{
    public class CurrencyExchangeDataFile : Jsonfile<List<CurrencyExchange>>
    {
        public CurrencyExchangeDataFile(IConfiguration configuration) : base(configuration, "currency-exchange.json")
        {
        }
    }
}
