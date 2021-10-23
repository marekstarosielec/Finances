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
        public async Task Initialize()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchPersistentContextAsync("browserContext", new BrowserTypeLaunchPersistentContextOptions { Headless = false });
            _page = await _browser.NewPageAsync();
        }

        public async Task NavigateTo(string url) => 
            await _page.GotoAsync(url);

        public async Task WaitForElement(string xpath) => 
            await _page.WaitForSelectorAsync(xpath, new PageWaitForSelectorOptions { });

        public async Task WaitForPage(string url) => 
            await _page.WaitForURLAsync(url, new PageWaitForURLOptions { Timeout = 0, WaitUntil = WaitUntilState.NetworkIdle });

        public async Task<bool> IsElementPresent(string xpath) => 
            (await _page.QuerySelectorAsync(xpath, new PageQuerySelectorOptions { })) != null;

        public async ValueTask DisposeAsync()
        {
            await _page.CloseAsync();
            await _browser.DisposeAsync();
            _playwright.Dispose();
        }

        public async Task Click(string xpath) => 
            await _page.ClickAsync(xpath, new PageClickOptions { });
    }
}
