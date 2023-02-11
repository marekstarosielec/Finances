using System;

namespace FinancesApi.Models
{
    public class Transaction
    {
        //Data scrapowania
        public DateTime? ScrappingDate { get; set; }
    
        //Status	
        public string Status { get; set; }

        //ScrapID
        public string Id { get; set; }

        public string Source { get; set; }

        //Data
        public DateTime Date { get; set; }

        //Konto
        public string Account { get; set; }

        //Kategoria
        public string Category { get; set; }

        //Kwota
        public decimal Amount { get; set; }

        //Tytuł przelewu
        public string Title { get; set; }

        //Opis operacji
        public string Description { get; set; }

        //Tekst tranzakcji
        public string Text { get; set; }

        public string BankInfo
        {
            get
            {
                var result = Text;
                if (!string.IsNullOrWhiteSpace(Title))
                    result += Environment.NewLine + Title;
                if (!string.IsNullOrWhiteSpace(Description))
                    result += Environment.NewLine + Description;
                return result;
            }
        }

        //Komentarz
        public string Comment { get; set; }

        public string Currency { get; set; }

        //Szczegóły
        public string Details { get; set; }

        //Osoba
        public string Person { get; set; }

        public string CaseName { get; set; }

        public string Settlement { get; set; }

        public string DocumentId { get; set; }

        public string DocumentCategory { get; set; }

        public string DocumentInvoiceNumber { get; set; }

        public int? DocumentNumber { get; set; }
    }
}
