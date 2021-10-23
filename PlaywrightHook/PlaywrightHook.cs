using MBankScrapper;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace PlaywrightHook
{
    public class PlaywrightHook : IBrowserHook, IAsyncDisposable
    {
        IPlaywright _playwright;
        IBrowserContext _browser;
        IPage _page;
        public async Task Start()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchPersistentContextAsync("browserContext", new BrowserTypeLaunchPersistentContextOptions { Headless = false });
            _page = await _browser.NewPageAsync();
        }

        public async Task NavigateTo(string url)
        {
            await _page.GotoAsync(url);
        }

        public async Task WaitForElement(string xpath)
        {
           // throw new NotImplementedException();
        }

        public async Task WaitForPage(string url)
        {
            await _page.WaitForURLAsync(url, new PageWaitForURLOptions { Timeout = 0,  WaitUntil = WaitUntilState.NetworkIdle });
        }


        public async ValueTask DisposeAsync()
        {
            await _page.CloseAsync();
            await _browser.DisposeAsync();
            _playwright.Dispose();
        }
    }
}
