using System;

namespace FinancesApi.Models
{
    public class Skoda
    {
        public string Id { get; set; }

        public DateTime Date { get; set; }
        
        public int Meter { get; set; }

        public string Comment { get; set; }
    }
}
