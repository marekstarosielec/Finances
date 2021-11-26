using System.Diagnostics;

namespace FinancesApi.Models
{
    [DebuggerDisplay("{Year}-{Month}")]
    public class StatisticsYearMonth
    {
        public int Year { get; set; }

        public int Month { get; set; }
    }
}
