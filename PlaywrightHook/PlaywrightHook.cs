using BrowserHook;
using Microsoft.Playwright;
using System;
using System.Text.RegularExpressions;
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
            _browser = await _playwright.Chromium.LaunchPersistentContextAsync("browserContext", new BrowserTypeLaunchPersistentContextOptions {  Headless = false});
            _page = await _browser.NewPageAsync();
        }

        public async Task NavigateTo(string url) => 
            await _page.GotoAsync(url);

        public async Task WaitForElement(string xpath, bool continueOnTimeout = false, int timeoutMilliseconds = 30000)
        {
            try
            {
                await _page.WaitForSelectorAsync(xpath, new PageWaitForSelectorOptions { Timeout = timeoutMilliseconds });
            }
            catch (System.TimeoutException) {
                if (!continueOnTimeout) throw;
            }
        }
           
        public async Task WaitForPage(string url) => 
            await _page.WaitForURLAsync(url, new PageWaitForURLOptions { Timeout = 0, WaitUntil = WaitUntilState.NetworkIdle  });

        public async Task WaitForPage(Regex regex) =>
            await _page.WaitForURLAsync(regex, new PageWaitForURLOptions { Timeout = 0, WaitUntil = WaitUntilState.NetworkIdle });

        public async Task<bool> IsElementPresent(string xpath) => 
            (await _page.QuerySelectorAsync(xpath, new PageQuerySelectorOptions { })) != null;

        public async Task<string> GetInnerText(string xpath) =>
            await _page.InnerTextAsync(xpath, new PageInnerTextOptions {  });

        public async ValueTask DisposeAsync()
        {
            await _page.CloseAsync();
            await _browser.DisposeAsync();
            _playwright.Dispose();
        }

        public async Task Click(string xpath) => 
            await _page.ClickAsync(xpath, new PageClickOptions {  });

        public async Task SetText(string xpath, string text)
        {
            await _page.TypeAsync(xpath, text, new PageTypeOptions { });
        }

        public async Task SendKey(string key, int count = 1)
        {
            for (var x = 0; x < count; x++)
                await _page.Keyboard.PressAsync(key, new KeyboardPressOptions { });
        }
    }
}
