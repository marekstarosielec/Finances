using FinancesBlazor.DataAccess;

namespace Finances.Entities.Electricity
{
    public class Electricity : IDataIdentifier
    {
        public string Id { get; set; }

        public DateTime Date { get; set; }

        public double Meter { get; set; }

        public string? Comment { get; set; }

        public Electricity(string id)
        {
            Id = id;
        }

    }
}
