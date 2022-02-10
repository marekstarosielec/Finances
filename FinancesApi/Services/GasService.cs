using FinancesApi.DataFiles;
using FinancesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancesApi.Controllers
{
    public interface IGasService
    {
        IList<Gas> GetGas();
        void SaveGas(Gas gas);
        void DeleteGas(string id);
    }

    public class GasService : IGasService
    {
        private readonly GasDataFile _gasDataFile;

        public GasService(GasDataFile gasDataFile)
        {
            _gasDataFile = gasDataFile;
        }

        public void DeleteGas(string id)
        {
            _gasDataFile.Load();
            _gasDataFile.Value.RemoveAll(a => string.Equals(id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            _gasDataFile.Save();
        }

        public IList<Gas> GetGas()
        {
            _gasDataFile.Load();
            return _gasDataFile.Value.OrderByDescending(a => a.Date).ToList();
        }

        public void SaveGas(Gas gas)
        {
            _gasDataFile.Load();
            var editedAccount = _gasDataFile.Value.FirstOrDefault(a => string.Equals(gas.Id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            if (editedAccount == null)
            {
                if (string.IsNullOrWhiteSpace(gas.Id))
                    gas.Id = Guid.NewGuid().ToString();
                _gasDataFile.Value.Add(gas);
            }
            else
            {
                editedAccount.Date = gas.Date;
                editedAccount.Meter = gas.Meter;
                editedAccount.Comment = gas.Comment;
            }
            _gasDataFile.Save();
        }
    }
}