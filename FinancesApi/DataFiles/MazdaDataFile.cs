using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FinancesApi.DataFiles
{
    public class MazdaDataFile : Jsonfile<List<Mazda>>
    {
        public MazdaDataFile(IConfiguration configuration) : base(configuration, "mazda.json")
        {
        }
    }
}
