using System.Collections.Generic;

namespace FinancesApi.Models
{
    public class StatisticsBills
    {
        public List<StatisticsYearMonth> YearMonth { get; set; }

        public List<string> Categories { get; set; }

        public Dictionary<StatisticsYearMonth, string> Amounts { get; set; }
    }
}
