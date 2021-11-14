﻿using FinancesApi.Services;
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
                        _balanceService.SaveBalance(new Models.Balance { Id = Guid.NewGuid().ToString(), Date=DateTime.Now.Date, Account = accountBalance.Title, Amount = accountBalance.Balance });
                    },
                    Transaction = transaction =>
                    {
                        DateTime.TryParseExact(transaction.Date, "dd'.'MM'.'yyyy", null, System.Globalization.DateTimeStyles.None, out var transactionDate);
                        _transactionService.SaveTransaction(new Models.Transaction { Id = transaction.Id, Details = transaction.Title, Amount = transaction.Amount, Account = transaction.Account, Date= transactionDate, Source = "mbank scrapper" });
                    }
                });
             
            }
            catch (Exception e)
            {

            }
            return Ok();
        }
    }
}
