using FinancesApi.DataFiles;
using FinancesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancesApi.Controllers
{
    public interface ISkodaService
    {
        IList<Skoda> GetSkoda();
        void SaveSkoda(Skoda skoda);
        void DeleteSkoda(string id);
    }

    public class SkodaService : ISkodaService
    {
        private readonly SkodaDataFile _skodaDataFile;

        public SkodaService(SkodaDataFile skodaDataFile)
        {
            _skodaDataFile = skodaDataFile;
        }

        public void DeleteSkoda(string id)
        {
            _skodaDataFile.Load();
            _skodaDataFile.Value.RemoveAll(a => string.Equals(id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            _skodaDataFile.Save();
        }

        public IList<Skoda> GetSkoda()
        {
            _skodaDataFile.Load();
            return _skodaDataFile.Value.OrderByDescending(a => a.Date).ToList();
        }

        public void SaveSkoda(Skoda skoda)
        {
            _skodaDataFile.Load();
            var editedAccount = _skodaDataFile.Value.FirstOrDefault(a => string.Equals(skoda.Id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            if (editedAccount == null)
            {
                if (string.IsNullOrWhiteSpace(skoda.Id))
                    skoda.Id = Guid.NewGuid().ToString();
                _skodaDataFile.Value.Add(skoda);
            }
            else
            {
                editedAccount.Date = skoda.Date;
                editedAccount.Meter = skoda.Meter;
                editedAccount.Comment = skoda.Comment;
            }
            _skodaDataFile.Save();
        }
    }
}