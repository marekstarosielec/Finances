using System.IO;

namespace FinancesApi.Models
{
    public class DataFile
    {
        public string FileName { get; set; }

        public string Location { get; set; }

        public string FileNameWithLocation { get => Path.Combine(Location, FileName); }
    }
}
