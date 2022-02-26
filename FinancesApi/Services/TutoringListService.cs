using FinancesApi.DataFiles;
using FinancesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancesApi.Controllers
{
    public interface ITutoringListService
    {
        IList<TutoringList> GetTutoringList();
        void SaveTutoringList(TutoringList tutoringList);
        void DeleteTutoringList(string id);
    }

    public class TutoringListService : ITutoringListService
    {
        private readonly TutoringListDataFile _tutoringListDataFile;

        public TutoringListService(TutoringListDataFile TutoringListDataFile)
        {
            _tutoringListDataFile = TutoringListDataFile;
        }

        public void DeleteTutoringList(string id)
        {
            _tutoringListDataFile.Load();
            _tutoringListDataFile.Value.RemoveAll(a => string.Equals(id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            _tutoringListDataFile.Save();
        }

        public IList<TutoringList> GetTutoringList()
        {
            _tutoringListDataFile.Load();
            return _tutoringListDataFile.Value.OrderByDescending(a => a.Title).ToList();
        }

        public void SaveTutoringList(TutoringList tutoringList)
        {
            _tutoringListDataFile.Load();
            var edited = _tutoringListDataFile.Value.FirstOrDefault(a => string.Equals(tutoringList.Id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            if (edited == null)
            {
                if (string.IsNullOrWhiteSpace(tutoringList.Id))
                    tutoringList.Id = Guid.NewGuid().ToString();
                _tutoringListDataFile.Value.Add(tutoringList);
            }
            else
            {
                edited.Title = tutoringList.Title;
                edited.Description = tutoringList.Description;
                edited.TransactionCategory = tutoringList.TransactionCategory;
            }
            _tutoringListDataFile.Save();
        }
    }
}