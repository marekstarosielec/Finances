using System;

namespace FinancesApi.Models
{
    public class CurrencyExchange
    {
        public string Id { get; set; }

        public DateTime Date { get; set; }

        public string Code { get; set; }

        public double Rate { get; set; } 
    }
}
