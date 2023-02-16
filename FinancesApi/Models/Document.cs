using System;

namespace FinancesApi.Models
{
    public class Document
    {
        public string Id { get; set; }
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public int? Pages { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string InvoiceNumber { get; set; }
        public string Company { get; set; }
        public string Person { get; set; }
        public string Car { get; set; }
        public string RelatedObject { get; set; }
        public string Guarantee { get; set; }
        public string CaseName { get; set; }
        public string Settlement { get; set; }

        public string TransactionId { get; set; }

        public string TransactionCategory { get; set; }

        public decimal? TransactionAmount { get; set; }

        public string TransactionCurrency { get; set; }

        public string TransactionBankInfo { get; set; }

        public string TransactionComment { get; set; }

        public decimal? Net { get; set; }
        public decimal? Vat { get; set; }
        public decimal? Gross { get; set; }
        public string Currency { get; set; }

    }
}
