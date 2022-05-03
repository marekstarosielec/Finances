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
    }
}
