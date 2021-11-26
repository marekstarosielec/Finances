using FinancesApi.Models;
using System;

namespace FinancesApi.Services
{
    public interface IStatisticsService
    {
        StatisticsBills GetBills();
        StatisticsAll GetAll();
    }

    public class StatisticsService: IStatisticsService
    {
        public StatisticsBills GetBills()
        {
            var result = new StatisticsBills();
            for (int x = 0; x < 48; x++)
            {
                var date = DateTime.Now.AddMonths(x * -1);
                result.YearMonth.Add(new StatisticsYearMonth { Year = date.Year, Month = date.Month });
            }
            result.Categories.Add("Gaz");
            result.Categories.Add("Prad");

            

            return result;
        }

        public StatisticsAll GetAll() => new StatisticsAll
        {
            Bills = GetBills()
        };
        
    }
}
