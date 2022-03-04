using FinancesApi.DataFiles;
using FinancesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancesApi.Controllers
{
    public interface ICaseListService
    {
        IList<CaseList> GetCaseList();
        void SaveCaseList(CaseList caseList);
        void DeleteCaseList(string id);
    }

    public class CaseListService : ICaseListService
    {
        private readonly CaseListDataFile _caseDataFile;

        public CaseListService(CaseListDataFile caseDataFile)
        {
            _caseDataFile = caseDataFile;
        }

        public void DeleteCaseList(string id)
        {
            _caseDataFile.Load();
            _caseDataFile.Value.RemoveAll(a => string.Equals(id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            _caseDataFile.Save();
        }

        public IList<CaseList> GetCaseList()
        {
            _caseDataFile.Load();
            return _caseDataFile.Value.OrderBy(a => a.Name).ToList();
        }

        public void SaveCaseList(CaseList caseList)
        {
            _caseDataFile.Load();
            var edited = _caseDataFile.Value.FirstOrDefault(a => string.Equals(caseList.Id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            if (edited == null)
            {
                if (string.IsNullOrWhiteSpace(caseList.Id))
                    caseList.Id = Guid.NewGuid().ToString();
                _caseDataFile.Value.Add(caseList);
            }
            else
            {
                edited.Name = caseList.Name;
                edited.Deleted = caseList.Deleted;
            }
            _caseDataFile.Save();
        }
    }
}