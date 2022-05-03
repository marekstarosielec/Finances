using System;

namespace FinancesApi.Models
{
    public class Water
    {
        public string Id { get; set; }

        public DateTime Date { get; set; }
        
        public double? Meter { get; set; }
        public double? Meter2 { get; set; }

        public int? Salt { get; set; }
        public string Comment { get; set; }
    }
}
