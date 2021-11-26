using FinancesApi.DataFiles;
using FinancesApi.Models;
using System;
using System.Linq;

namespace FinancesApi.Services
{
    public interface IStatisticsService
    {
        StatisticsBills GetBills();
        StatisticsAll GetAll();
    }

    public class StatisticsService: IStatisticsService
    {
        private readonly TransactionsDataFile _transactionsDataFile;

        public StatisticsService(TransactionsDataFile transactionsDataFile)
        {
            _transactionsDataFile = transactionsDataFile;
        }

        public StatisticsBills GetBills()
        {
            var result = new StatisticsBills();
            for (int x = 0; x < 48; x++)
            {
                var date = DateTime.Now.AddMonths(x * -1);
                result.Periods.Add(new StatisticsYearMonth { Year = date.Year, Month = date.Month });
            }
            result.Categories.Add("Gaz");
            result.Categories.Add("Prąd");
            result.Categories.Add("Internet");
            result.Categories.Add("Marek ubezpieczenie na życie");
            result.Categories.Add("Mazda leasing");
            result.Categories.Add("Skoda rata");
            result.Categories.Add("Telefon");
            result.Categories.Add("Woda");
            result.Categories.Add("Śmieci");

            _transactionsDataFile.Load();
            var amounts = _transactionsDataFile.Value.Where(t => result.Categories.Contains(t.Category)).GroupBy(t => new
            {
                t.Category,
                t.Date.Year,
                t.Date.Month
            }).Select(g => new StatisticsBill(
                new StatisticsYearMonth
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month
                },
                g.Key.Category,
                g.ToList().Sum(t => t.Amount)
            )).ToList();

            foreach (var period in result.Periods)
                foreach (var category in result.Categories)
                    result.Amounts.Add(new StatisticsBill(period, category, amounts.FirstOrDefault(a => a.Period.Year == period.Year && a.Period.Month == period.Month && a.Category == category).Amount));

            foreach (var period in result.Periods)
            {
                var sums = amounts.Where(a => a.Period.Year == period.Year && a.Period.Month == period.Month).Where(a => a.Amount != null).ToList();
                var calculatedSum = sums.Count > 0 ? sums.Sum(a => a.Amount) : null;
                result.Amounts.Add(new StatisticsBill(period, null, calculatedSum));
            }
            return result;
        }

        public StatisticsAll GetAll() => new StatisticsAll
        {
            Bills = GetBills()
        };
        
    }
}
