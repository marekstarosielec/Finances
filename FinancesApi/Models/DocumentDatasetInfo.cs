using System;


namespace FinancesApi.Models
{
    public class DocumentDatasetInfo
    {
        public DatasetState State { get; set; }

        public DateTime LastCloseDate { get; set; }

        public string Error { get; set; }
    }
}
