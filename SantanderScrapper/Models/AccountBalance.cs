namespace SantanderScrapper.Models
{
    public class AccountBalance
    {
        public string Title { get; set; }
        public string Iban { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
    }
}
