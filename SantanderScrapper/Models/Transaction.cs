using System;

namespace SantanderScrapper.Models
{
    public class Transaction
    {
        public string Account { get; set; }
        public string Id { get; set; }

        public string Title { get; set; }

        public string Currency { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }
    }
}
