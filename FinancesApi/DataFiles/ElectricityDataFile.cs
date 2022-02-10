using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FinancesApi.DataFiles
{
    public class ElectricityDataFile : Jsonfile<List<Electricity>>
    {
        public ElectricityDataFile(IConfiguration configuration) : base(configuration, "electricity.json")
        {
        }
    }
}
