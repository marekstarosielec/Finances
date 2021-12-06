using FinancesApi.DataFiles;
using FinancesApi.Models;
using System.Collections.Generic;

namespace FinancesApi.Services
{
    public interface ICurrenciesService
    {
        IList<Currency> GetCurrencies();
    }

    public class CurrenciesService : ICurrenciesService
    {
        private readonly CurrenciesDataFile _currenciesDataFile;

        public CurrenciesService(CurrenciesDataFile currenciesDataFile)
        {
            _currenciesDataFile = currenciesDataFile;
        }

        public IList<Currency> GetCurrencies()
        {
            _currenciesDataFile.Load();
            return _currenciesDataFile.Value;
        }
    }
}
