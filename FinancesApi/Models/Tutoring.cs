using System;

namespace FinancesApi.Models
{
    public class Tutoring
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public DateTime Date { get; set; }

        public double Count { get; set; }

        public string Comment { get; set; }
    }
}
