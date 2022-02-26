using FinancesApi.DataFiles;
using FinancesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancesApi.Controllers
{
    public interface ITutoringService
    {
        IList<Tutoring> GetTutoring();
        void SaveTutoring(Tutoring tutoring);
        void DeleteTutoring(string id);
    }

    public class TutoringService : ITutoringService
    {
        private readonly TutoringDataFile _tutoringDataFile;

        public TutoringService(TutoringDataFile TutoringDataFile)
        {
            _tutoringDataFile = TutoringDataFile;
        }

        public void DeleteTutoring(string id)
        {
            _tutoringDataFile.Load();
            _tutoringDataFile.Value.RemoveAll(a => string.Equals(id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            _tutoringDataFile.Save();
        }

        public IList<Tutoring> GetTutoring()
        {
            _tutoringDataFile.Load();
            return _tutoringDataFile.Value.OrderByDescending(a => a.Title).ToList();
        }

        public void SaveTutoring(Tutoring tutoring)
        {
            _tutoringDataFile.Load();
            var edited = _tutoringDataFile.Value.FirstOrDefault(a => string.Equals(tutoring.Id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            if (edited == null)
            {
                if (string.IsNullOrWhiteSpace(tutoring.Id))
                    tutoring.Id = Guid.NewGuid().ToString();
                _tutoringDataFile.Value.Add(tutoring);
            }
            else
            {
                edited.Title = tutoring.Title;
                edited.Date = tutoring.Date;
                edited.Count = tutoring.Count;
                edited.Comment = tutoring.Comment;
            }
            _tutoringDataFile.Save();
        }
    }
}