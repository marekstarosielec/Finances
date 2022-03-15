using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FinancesApi.DataFiles
{
    public class SettlementDataFile : Jsonfile<List<Settlement>>
    {
        public SettlementDataFile(IConfiguration configuration) : base(configuration, "settlement.json")
        {
        }
    }
}
