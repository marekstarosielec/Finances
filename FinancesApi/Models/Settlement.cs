using System;

namespace FinancesApi.Models
{
    public class Settlement
    {
        public string Id { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public int Quarter { get; set; }

        public string Title { get; set; }

        public double IncomeGrossPln { get; set; }
        public double IncomeGrossEur { get; set; }

        public double ExchangeRatio { get; set; }

        public double BalanceAccountPln { get; set; }
       
        public double Zus { get; set; }

        public double Pit { get; set; }

        public double Vat { get; set; }

        public double Reserve { get; set; }

        public double Total { get; set; }

        public double Revenue { get; set; }

        public string Comment { get; set; }

        public bool Closed { get; set; }
    }
}
