using Finances.DataAccess;
using Finances.DependencyInjection;

namespace Finances.Entities.Electricity;

public class ElectricityService : BaseListService<Electricity>, IInjectAsSingleton
{
    public ElectricityService(IConfiguration configuration): base(new ElectricityDataFile(configuration)) { }
}