using System;

namespace FinancesApi.Models
{
    public class Gas
    {
        public string Id { get; set; }

        public DateTime Date { get; set; }
        
        public double Meter { get; set; }

        public string Comment { get; set; }
    }
}
