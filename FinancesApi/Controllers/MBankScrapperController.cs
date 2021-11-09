using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FinancesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MBankScrapperController : ControllerBase
    {
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
                        Console.WriteLine(accountBalance.Title);
                        Console.WriteLine(accountBalance.Iban);
                        Console.WriteLine(accountBalance.Balance);
                        Console.WriteLine(accountBalance.Currency);
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
