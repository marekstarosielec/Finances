using FinancesApi.DataFiles;
using FinancesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancesApi.Controllers
{
    public interface IWaterService
    {
        IList<Water> Get();
        void Save(Water model);
        void Delete(string id);
    }

    public class WaterService : IWaterService
    {
        private readonly WaterDataFile _dataFile;

        public WaterService(WaterDataFile dataFile)
        {
            _dataFile = dataFile;
        }

        public void Delete(string id)
        {
            _dataFile.Load();
            _dataFile.Value.RemoveAll(a => string.Equals(id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            _dataFile.Save();
        }

        public IList<Water> Get()
        {
            _dataFile.Load();
            return _dataFile.Value.OrderByDescending(a => a.Date).ToList();
        }

        public void Save(Water model)
        {
            _dataFile.Load();
            var edited = _dataFile.Value.FirstOrDefault(a => string.Equals(model.Id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            if (edited == null)
            {
                if (string.IsNullOrWhiteSpace(model.Id))
                    model.Id = Guid.NewGuid().ToString();
                _dataFile.Value.Add(model);
            }
            else
            {
                edited.Date = model.Date;
                edited.Meter = model.Meter;
                edited.Meter2 = model.Meter2;
                edited.Salt = model.Salt;
                edited.Comment = model.Comment;
            }
            _dataFile.Save();
        }
    }
}