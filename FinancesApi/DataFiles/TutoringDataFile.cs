using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FinancesApi.DataFiles
{
    public class TutoringDataFile : Jsonfile<List<Tutoring>>
    {
        public TutoringDataFile(IConfiguration configuration) : base(configuration, "tutoring.json")
        {
        }
    }
}
