﻿namespace FinancesApi.Models
{
    public class TransactionCategory
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public bool Deleted { get; set; }
        public int UsageIndex { get; set; }
    }
}
