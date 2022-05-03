using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FinancesApi.DataFiles
{
    public class WaterDataFile : Jsonfile<List<Water>>
    {
        public WaterDataFile(IConfiguration configuration) : base(configuration, "water.json")
        {
        }
    }
}
