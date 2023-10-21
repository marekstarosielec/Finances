using FinancesBlazor.DataAccess;

namespace Finances.Entities.Electricity;

public class ElectricityDataFile : JsonListFile<Electricity>
{
    public ElectricityDataFile(IConfiguration configuration) : base(configuration, "electricity.json") { }
}
