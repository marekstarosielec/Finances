using FinancesApi.DataFiles;
using FinancesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancesApi.Services
{
    public interface ICurrencyExchangeService
    {
        IList<CurrencyExchange> Get();
        void Save(CurrencyExchange currencyExchange);
        void Delete(string id);
    }

    public class CurrencyExchangeService : ICurrencyExchangeService
    {
        private readonly CurrencyExchangeDataFile _currencyExchangeDataFile;
        private object _saving = new object();
        public CurrencyExchangeService(CurrencyExchangeDataFile currencyExchangeDataFile)
        {
            _currencyExchangeDataFile = currencyExchangeDataFile;
        }

        public void Delete(string id)
        {
            _currencyExchangeDataFile.Load();
            _currencyExchangeDataFile.Value.RemoveAll(a => string.Equals(id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            _currencyExchangeDataFile.Save();
        }

        public IList<CurrencyExchange> Get()
        {
            _currencyExchangeDataFile.Load();
            return _currencyExchangeDataFile.Value;
        }

        public void Save(CurrencyExchange currencyExchange)
        {
            lock(_saving)
            {
                _currencyExchangeDataFile.Load();
                var editedAccount = _currencyExchangeDataFile.Value.FirstOrDefault(a => string.Equals(currencyExchange.Id, a.Id, StringComparison.InvariantCultureIgnoreCase));
                if (editedAccount == null)
                {
                    if (string.IsNullOrWhiteSpace(currencyExchange.Id))
                        currencyExchange.Id = Guid.NewGuid().ToString();
                    _currencyExchangeDataFile.Value.Add(currencyExchange);
                }
                else
                {
                    editedAccount.Date = currencyExchange.Date;
                    editedAccount.Code = currencyExchange.Code;
                    editedAccount.Rate = currencyExchange.Rate;
                }
                _currencyExchangeDataFile.Save();
            }
        }
    }
}
