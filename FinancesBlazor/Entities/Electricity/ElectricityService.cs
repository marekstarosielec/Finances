using Finances.DataAccess;
using Finances.DependencyInjection;

namespace Finances.Entities.Electricity;

public class ElectricityService : BaseListService<Electricity, DateTime>, IInjectAsSingleton
{
    public ElectricityService(IConfiguration configuration): base(new ElectricityDataFile(configuration), e => e.Date, ascending: false)
    {
      
    }

    //public void SaveElectricity(Electricity electricity)
    //{
    //    _electricityDataFile.Load();
    //    var editedAccount = _electricityDataFile.Value.FirstOrDefault(a => string.Equals(electricity.Id, a.Id, StringComparison.InvariantCultureIgnoreCase));
    //    if (editedAccount == null)
    //    {
    //        if (string.IsNullOrWhiteSpace(electricity.Id))
    //            electricity.Id = Guid.NewGuid().ToString();
    //        _electricityDataFile.Value.Add(electricity);
    //    }
    //    else
    //    {
    //        editedAccount.Date = electricity.Date;
    //        editedAccount.Meter = electricity.Meter;
    //        editedAccount.Comment = electricity.Comment;
    //    }
    //    _electricityDataFile.Save();
    //}
}