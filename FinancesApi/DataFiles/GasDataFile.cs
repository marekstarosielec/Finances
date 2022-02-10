using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FinancesApi.DataFiles
{
    public class GasDataFile : Jsonfile<List<Gas>>
    {
        public GasDataFile(IConfiguration configuration) : base(configuration, "gas.json")
        {
        }
    }
}
