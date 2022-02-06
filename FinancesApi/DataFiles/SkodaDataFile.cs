using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FinancesApi.DataFiles
{
    public class SkodaDataFile : Jsonfile<List<Skoda>>
    {
        public SkodaDataFile(IConfiguration configuration) : base(configuration, "skoda.json")
        {
        }
    }
}
