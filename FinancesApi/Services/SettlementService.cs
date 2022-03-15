using FinancesApi.DataFiles;
using FinancesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancesApi.Controllers
{
    public interface ISettlementService
    {
        IList<Settlement> Get();
        void Save(Settlement model);
        void Delete(string id);
    }

    public class SettlementService : ISettlementService
    {
        private readonly SettlementDataFile _settlementDataFile;

        public SettlementService(SettlementDataFile settlementDataFile)
        {
            _settlementDataFile = settlementDataFile;
        }

        
        public void Delete(string id)
        {
            _settlementDataFile.Load();
            _settlementDataFile.Value.RemoveAll(a => string.Equals(id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            _settlementDataFile.Save();
        }

        public IList<Settlement> Get()
        {
            _settlementDataFile.Load();
            return _settlementDataFile.Value.OrderByDescending(m => m.Year).ThenByDescending(m => m.Month).ToList();
        }

        public void Save(Settlement model)
        {
            _settlementDataFile.Load();
            var edited = _settlementDataFile.Value.FirstOrDefault(m => string.Equals(model.Id, m.Id, StringComparison.InvariantCultureIgnoreCase));
            if (edited == null)
            {
                if (string.IsNullOrWhiteSpace(model.Id))
                    model.Id = Guid.NewGuid().ToString();
                _settlementDataFile.Value.Add(model);
            }
            else
            {
                edited.Year = model.Year;
                edited.Month = model.Month;
                edited.Title = model.Title; 
                edited.IncomeGrossPln = model.IncomeGrossPln;
                edited.BalanceAccountPln = model.BalanceAccountPln;
                edited.Zus = model.Zus;
                edited.Pit = model.Pit;
                edited.Vat = model.Vat;
                edited.Reserve = model.Reserve;
                edited.Total = model.Total;
                edited.Revenue = model.Revenue;
                edited.Comment = model.Comment;
            }
            _settlementDataFile.Save();
        }
    }
}