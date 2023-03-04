using System;

namespace FinancesApi.Models
{
    public class Incoming
    {
        public string FullFileName { get; set; }
        public string FileName { get; set; }
        public string SortableFileName { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
