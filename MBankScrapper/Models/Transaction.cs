namespace MBankScrapper.Models
{
    public class Transaction
    {
        public string Account { get; set; }

        public string Id { get; set; }

        public string Text { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Currency { get; set; }

        public string Date { get; set; }

        public decimal Amount { get; set; }
    }
}
