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

        public MBankScrapperController(IBalanceService balanceService)
        {
            _balanceService = balanceService;
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
