using FinancesApi.DataFiles;
using FinancesApi.Models;
using FinancesApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancesApi.Controllers
{
    public interface ISettlementService
    {
        IList<Settlement> Get();
        void Save(Settlement model);
        void Delete(string id);
    }

    public class SettlementService : ISettlementService
    {
        private readonly SettlementDataFile _settlementDataFile;
        private readonly ITransactionsService _transactionsService;
        private readonly ICurrencyExchangeService _currencyExchangeService;
        private readonly IDocumentService _documentService;

        public SettlementService(SettlementDataFile settlementDataFile, ITransactionsService transactionsService, ICurrencyExchangeService currencyExchangeService, IDocumentService documentService)
        {
            _settlementDataFile = settlementDataFile;
            _transactionsService = transactionsService;
            _currencyExchangeService = currencyExchangeService;
            _documentService = documentService;
        }

        
        public void Delete(string id)
        {
            _settlementDataFile.Load();
            _settlementDataFile.Value.RemoveAll(a => string.Equals(id, a.Id, StringComparison.InvariantCultureIgnoreCase));
            _settlementDataFile.Save();
        }

        public IList<Settlement> Get()
        {
            _settlementDataFile.Load();
            var result = _settlementDataFile.Value.OrderByDescending(m => m.Year).ThenByDescending(m => m.Month).ToList();
            var lastDate = new DateTime(result[0].Year, result[0].Month, 1);
            var currentDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            var transactions = _transactionsService.GetTransactions();
            var documents = _documentService.GetDocuments();
            var exchanges = _currencyExchangeService.Get();
            result.ForEach(s => { 
                s.EurConvertedToPln = (double) Math.Abs(transactions.Where(t => t.Settlement == s.Title && t.BankInfo == "OBCIĄŻ. NATYCH. TRANSAKCJA WALUT.").Sum(t => t.Amount));
                s.PlnReceivedFromConvertion = (double)transactions.Where(t => t.Settlement == s.Title && t.BankInfo == "UZNANIE NATYCH. TRANSAKCJA WALUT.").Sum(t => t.Amount);
                s.ConvertionExchangeRatio = s.EurConvertedToPln > 0 ? Math.Round(s.PlnReceivedFromConvertion / s.EurConvertedToPln, 4) : 0;

                var previousSettlement = _settlementDataFile.Value.Where(search => search.Year == s.Year && search.Quarter == s.Quarter && search.Month == s.Month - 1).FirstOrDefault();
                if (previousSettlement == null)
                    previousSettlement = new Settlement();

                var e = exchanges.Where(e => e.Date.Year == s.Year && e.Date.Month == s.Month).OrderByDescending(e => e.Date).FirstOrDefault();
                if (e != null)
                {
                    s.PitAndVatPln = s.Pit + s.Vat;
                    s.PitAndVatEur = Math.Round(s.PitAndVatPln / e.Rate, 2);

                    s.PitMonth = s.Pit - previousSettlement.Pit;
                    s.VatMonth = s.Vat - previousSettlement.Vat;
                    s.PitAndVatMonthPln = s.PitMonth + s.VatMonth;
                    s.PitAndVatMonthEur = Math.Round(s.PitAndVatMonthPln / e.Rate, 2);

                    s.RemainingEur = s.IncomeGrossEur - s.EurConvertedToPln;

                    var zusTransaction = transactions.FirstOrDefault(t => t.Category == "Marek ZUS" && t.Settlement == s.Title);
                    if (zusTransaction == null)
                        s.ZusPaid = 0;
                    else if (Math.Abs((double)zusTransaction.Amount) == s.Zus)
                        s.ZusPaid = 1;
                    else
                        s.ZusPaid = 2;

                    var upoJpkDocument = documents.FirstOrDefault(d => d.Description?.StartsWith("UPO JPK ")==true && d.Settlement == s.Title);
                    s.JpkSent = upoJpkDocument == null ? 0 : 1;
                    
                    if (s.Month == 3 || s.Month == 6 || s.Month == 9 || s.Month == 12)
                    {
                        var pitTransaction = transactions.FirstOrDefault(t => t.Category == "Marek podatek dochodowy" && t.Settlement == s.Title);
                        if (pitTransaction == null)
                            s.PitPaid = 0;
                        else if (Math.Abs((double)pitTransaction.Amount) == s.Pit)
                            s.PitPaid = 1;
                        else
                            s.PitPaid = 2;

                        var vatTransaction = transactions.FirstOrDefault(t => t.Category == "Marek VAT" && t.Settlement == s.Title);
                        if (vatTransaction == null)
                            s.VatPaid = 0;
                        else if (Math.Abs((double)vatTransaction.Amount) == s.Vat)
                            s.VatPaid = 1;
                        else
                            s.VatPaid = 2;
                    }
                    else
                    {
                        s.PitPaid = 3;
                        s.VatPaid = 3;
                    }
                }
            });


            if (lastDate >= currentDate)
                return result;
            
            _settlementDataFile.Value.Add(new Settlement
            {
                Id = Guid.NewGuid().ToString(),
                Year = currentDate.Year,
                Month = currentDate.Month,
                Quarter = currentDate.Month switch
                {
                    1 or 2 or 3 => 1,
                    4 or 5 or 6 => 2,
                    7 or 8 or 9 => 3,
                    _ => 4
                },
                IncomeGrossEur = 0,
                Pit = result[0].Pit,
                Vat = result[0].Vat,
                Title = $"{currentDate.Year}-{currentDate.Month.ToString().PadLeft(2, '0')}",
                Zus = result[0].Zus,
                Reserve = result[0].Reserve
            });
            _settlementDataFile.Save();
            
            return _settlementDataFile.Value.OrderByDescending(m => m.Year).ThenByDescending(m => m.Month).ToList();
        }

        public void Save(Settlement model)
        {
            _settlementDataFile.Load();
            var edited = _settlementDataFile.Value.FirstOrDefault(m => string.Equals(model.Id, m.Id, StringComparison.InvariantCultureIgnoreCase));
            if (edited == null)
            {
                if (string.IsNullOrWhiteSpace(model.Id))
                    model.Id = Guid.NewGuid().ToString();
                _settlementDataFile.Value.Add(model);
            }
            else
            {
                edited.Year = model.Year;
                edited.Month = model.Month;
                edited.Quarter = model.Quarter;
                edited.Title = model.Title; 
                edited.IncomeGrossPln = model.IncomeGrossPln;
                edited.IncomeGrossEur = model.IncomeGrossEur;
                edited.RemainingEur = model.RemainingEur;
                edited.ExchangeRatio = model.ExchangeRatio;
                edited.BalanceAccountPln = model.BalanceAccountPln;
                edited.Zus = model.Zus;
                edited.Pit = model.Pit;
                edited.Vat = model.Vat;
                edited.Reserve = model.Reserve;
                edited.Total = model.Total;
                edited.Revenue = model.Revenue;
                edited.Comment = model.Comment;
                edited.Closed = model.Closed;
            }
            _settlementDataFile.Save();
        }
    }
}