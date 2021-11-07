using System;

namespace FinancesApi.Models
{
    public class Balance
    {
        public string Id { get; set; }
        
        public DateTime Date { get; set; }

        public string Account { get; set; }
        public decimal Amount { get; set; }
    }
}
