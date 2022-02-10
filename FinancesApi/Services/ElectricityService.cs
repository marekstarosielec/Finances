using FinancesApi.DataFiles;
using FinancesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancesApi.Controllers
{
    public interface IElectricityService
    {
        IList<Electricity> GetElectricity();
        void SaveElectricity(Electricity electricity);
        void DeleteElectricity(string id);
    }

    public class ElectricityService : IElectricityService
    {
        private readonly ElectricityDataFile _electricityDataFile;

        public ElectricityService(ElectricityDataFile electricityDataFile)
        {
            _electricityDataFile = electricityDataFile;
        }

        public void DeleteElectricity(string id)
        {
            _electricityDataFile.Load();
            _electricityDataFile.Value.RemoveAll(a => string.Equals(id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            _electricityDataFile.Save();
        }

        public IList<Electricity> GetElectricity()
        {
            _electricityDataFile.Load();
            return _electricityDataFile.Value.OrderByDescending(a => a.Date).ToList();
        }

        public void SaveElectricity(Electricity electricity)
        {
            _electricityDataFile.Load();
            var editedAccount = _electricityDataFile.Value.FirstOrDefault(a => string.Equals(electricity.Id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            if (editedAccount == null)
            {
                if (string.IsNullOrWhiteSpace(electricity.Id))
                    electricity.Id = Guid.NewGuid().ToString();
                _electricityDataFile.Value.Add(electricity);
            }
            else
            {
                editedAccount.Date = electricity.Date;
                editedAccount.Meter = electricity.Meter;
                editedAccount.Comment = electricity.Comment;
            }
            _electricityDataFile.Save();
        }
    }
}