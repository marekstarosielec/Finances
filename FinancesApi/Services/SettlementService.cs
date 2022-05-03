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
            var result = _settlementDataFile.Value.OrderByDescending(m => m.Year).ThenByDescending(m => m.Month).ToList();
            var lastDate = new DateTime(result[0].Year, result[0].Month, 1);
            var currentDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
            if (lastDate >= currentDate)
                return result;
            
            _settlementDataFile.Value.Add(new Settlement
            {
                Id = Guid.NewGuid().ToString(),
                Year = currentDate.Year,
                Month = currentDate.Month,
                Quarter = currentDate.Month switch
                {
                    1 or 2 or 3 => 1,
                    4 or 5 or 6 => 2,
                    7 or 8 or 9 => 3,
                    _ => 4
                },
                IncomeGrossEur = result[0].IncomeGrossEur,
                Title = $"{currentDate.Year}-{currentDate.Month.ToString().PadLeft(2, '0')}",
                Zus = result[0].Zus,
                Reserve = result[0].Reserve
            });
            _settlementDataFile.Save();
            
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
                edited.Quarter = model.Quarter;
                edited.Title = model.Title; 
                edited.IncomeGrossPln = model.IncomeGrossPln;
                edited.IncomeGrossEur = model.IncomeGrossEur;
                edited.ExchangeRatio = model.ExchangeRatio;
                edited.BalanceAccountPln = model.BalanceAccountPln;
                edited.Zus = model.Zus;
                edited.Pit = model.Pit;
                edited.Vat = model.Vat;
                edited.Reserve = model.Reserve;
                edited.Total = model.Total;
                edited.Revenue = model.Revenue;
                edited.Comment = model.Comment;
                edited.Closed = model.Closed;
            }
            _settlementDataFile.Save();
        }
    }
}