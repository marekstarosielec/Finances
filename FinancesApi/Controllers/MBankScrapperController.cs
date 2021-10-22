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
              
                using var playwright = await Playwright.CreateAsync();
                  
                await using var browser = await playwright.Chromium.LaunchAsync( new BrowserTypeLaunchOptions { Headless = false });
                
                var page = await browser.NewPageAsync();
                
                await page.GotoAsync("https://playwright.dev/dotnet");
                await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot.png" });
            }
            catch (Exception e)
            {

            }
            return Ok();
        }
    }
}
