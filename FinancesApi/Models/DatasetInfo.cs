using System;


namespace FinancesApi.Models
{
    public class DatasetInfo
    {
        public DatasetState State { get; set; }

        public DateTime LastCloseDate { get; set; }

        public string Message { get; set; }
    }
}
