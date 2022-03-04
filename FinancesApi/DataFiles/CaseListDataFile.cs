using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FinancesApi.DataFiles
{
    public class CaseListDataFile : Jsonfile<List<CaseList>>
    {
        public CaseListDataFile(IConfiguration configuration): base(configuration, "case-list.json")
        {
        }
    }
}
