using FinancesApi.DataFiles;
using FinancesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancesApi.Controllers
{
    public interface IMazdaService
    {
        IList<Mazda> GetMazda();
        void SaveMazda(Mazda mazda);
        void DeleteMazda(string id);
    }

    public class MazdaService : IMazdaService
    {
        private readonly MazdaDataFile _mazdaDataFile;

        public MazdaService(MazdaDataFile mazdaDataFile)
        {
            _mazdaDataFile = mazdaDataFile;
        }

        public void DeleteMazda(string id)
        {
            _mazdaDataFile.Load();
            _mazdaDataFile.Value.RemoveAll(a => string.Equals(id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            _mazdaDataFile.Save();
        }

        public IList<Mazda> GetMazda()
        {
            _mazdaDataFile.Load();
            return _mazdaDataFile.Value.OrderByDescending(a => a.Date).ToList();
        }

        public void SaveMazda(Mazda mazda)
        {
            _mazdaDataFile.Load();
            var editedAccount = _mazdaDataFile.Value.FirstOrDefault(a => string.Equals(mazda.Id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            if (editedAccount == null)
            {
                if (string.IsNullOrWhiteSpace(mazda.Id))
                    mazda.Id = Guid.NewGuid().ToString();
                _mazdaDataFile.Value.Add(mazda);
            }
            else
            {
                editedAccount.Date = mazda.Date;
                editedAccount.Meter = mazda.Meter;
                editedAccount.Comment = mazda.Comment;
            }
            _mazdaDataFile.Save();
        }
    }
}