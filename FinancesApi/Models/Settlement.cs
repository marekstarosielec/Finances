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
        public double RemainingEur { get; set; }

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

        public double EurConvertedToPln { get; set; }
        public double PlnReceivedFromConvertion { get; set; }
        public double ConvertionExchangeRatio { get; set; }
        public double PitAndVatPln { get; set; }
        public double PitAndVatEur { get; set; }
        public double PitMonth { get; set; }
        public double VatMonth { get; set; }
        public double PitAndVatMonthPln { get; set; }
        public double PitAndVatMonthEur { get; set; }

        public int ZusPaid { get; set; }
        public int PitPaid { get; set; }
        public int VatPaid { get; set; }
    }
}
