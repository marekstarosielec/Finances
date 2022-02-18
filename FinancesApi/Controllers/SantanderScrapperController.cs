using FinancesApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FinancesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SantanderScrapperController : ControllerBase
    {
        private readonly IBalanceService _balanceService;
        private readonly ITransactionsService _transactionService;

        public SantanderScrapperController(IBalanceService balanceService, ITransactionsService transactionService)
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
                var santander = new SantanderScrapper.Santander();
                await santander.StartScrapping(hook, new SantanderScrapper.ActionSet {
                    AccountBalance = accountBalance =>
                    {
                       _balanceService.SaveBalance(new Models.Balance { 
                           Id = Guid.NewGuid().ToString(), 
                           Date=DateTime.Now.Date, 
                           Account = accountBalance.Title, 
                           Amount = accountBalance.Balance, 
                           Currency = accountBalance.Currency });
                    },
                    Transaction = transaction =>
                    {
                        var transactionModel = new Models.Transaction
                        {
                            Id = transaction.Id,
                            Title = transaction.Title,
                            Amount = transaction.Amount,
                            Account = transaction.Account,
                            Date = transaction.Date,
                            Source = "santander scrapper",
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
