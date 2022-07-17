using FinancesApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FinancesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MBankScrapperController : ControllerBase
    {
        private readonly IBalanceService _balanceService;
        private readonly ITransactionsService _transactionService;

        public MBankScrapperController(IBalanceService balanceService, ITransactionsService transactionService)
        {
            _balanceService = balanceService;
            _transactionService = transactionService;
        }

        [HttpPost]
        public async Task<IActionResult> Start()
        {
            try
            {
                await using var hook = new PlaywrightHook.PlaywrightHook();
                var mBank = new MBankScrapper.MBank();
                await mBank.StartScrapping(hook, new MBankScrapper.ActionSet {
                    AccountBalance = accountBalance =>
                    {
                        _balanceService.SaveBalance(new Models.Balance { Id = Guid.NewGuid().ToString(), Date=DateTime.Now.Date, Account = accountBalance.Title, Amount = accountBalance.Balance, Currency = accountBalance.Currency });
                    },
                    Transaction = transaction =>
                    {
                        if (transaction.Date.IndexOf(".") == 1)
                            transaction.Date = "0" + transaction.Date;
                        if (transaction.Date.LastIndexOf(".") == 4)
                            transaction.Date = transaction.Date.Substring(0,3) + "0" + transaction.Date.Substring(3);

                        DateTime.TryParseExact(transaction.Date, "dd'.'MM'.'yyyy", null, System.Globalization.DateTimeStyles.AssumeUniversal, out var transactionDate);

                        var transactionModel = new Models.Transaction
                        {
                            Id = transaction.Id,
                            Text = transaction.Text.Replace("Kliknij i sprawdź szczegóły.", string.Empty).Trim(),
                            Title = transaction.Title.Replace("Kliknij i sprawdź szczegóły.", string.Empty).Trim(),
                            Description = transaction.Description.Replace("Kliknij i sprawdź szczegóły.", string.Empty).Trim(),
                            Amount = transaction.Amount,
                            Account = transaction.Account,
                            Date = transactionDate.ToUniversalTime(),
                            Source = "mbank scrapper",
                            Currency = transaction.Currency,
                            ScrappingDate = DateTime.Now
                        };
                        transactionModel.Category = _transactionService.GetAutoCategory(transactionModel);

                        _transactionService.SaveTransaction(
                            transactionModel, false);
                    }
                });
                //_transactionService.ApplyAutoCategories();
            }
            catch (Exception e)
            {

            }
            return Ok();
        }
    }
}
