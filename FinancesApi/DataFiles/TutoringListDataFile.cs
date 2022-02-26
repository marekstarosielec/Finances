using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FinancesApi.DataFiles
{
    public class TutoringListDataFile : Jsonfile<List<TutoringList>>
    {
        public TutoringListDataFile(IConfiguration configuration) : base(configuration, "tutoring-list.json")
        {
        }
    }
}
