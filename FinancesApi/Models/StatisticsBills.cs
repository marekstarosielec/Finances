using System.Collections.Generic;

namespace FinancesApi.Models
{
    public class StatisticsBills
    {
        public List<StatisticsYearMonth> Periods { get; set; } = new List<StatisticsYearMonth>();

        public List<string> Categories { get; set; } = new List<string>();

        public List<StatisticsBill> Amounts { get; set; } = new List<StatisticsBill>();


    }

    public readonly struct StatisticsBill
    {
        public readonly StatisticsYearMonth Period { get; }
        public readonly string Category { get; }
        public readonly decimal? Amount { get; }

        public StatisticsBill(StatisticsYearMonth period, string category, decimal? amount)
        {
            Period = period;
            Category = category;
            Amount = amount;
        }
    }
}
