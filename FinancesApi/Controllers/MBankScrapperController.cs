using Microsoft.AspNetCore.Mvc;
using Microsoft.Playwright;
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
                await hook.Start();
                await mBank.Login(hook);
                await mBank.SwitchToPrivateProfile(hook);
            }
            catch (Exception e)
            {

            }
            return Ok();
        }
    }
}
