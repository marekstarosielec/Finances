using Finances.DataAccess;
using Finances.DependencyInjection;

namespace Finances.Entities.Electricity;

//public class ElectricityService : BaseListService<Electricity>, IInjectAsSingleton
//{
//    public ElectricityService(IConfiguration configuration): base(new ElectricityDataFile(configuration)) { }
//}

public class ElectricityService2 : BaseListService, IInjectAsSingleton
{
    public ElectricityService2(IConfiguration configuration) : base(new ElectricityDataFile2(configuration)) { }
}