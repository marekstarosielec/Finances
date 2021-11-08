using BrowserHook;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MBankScrapper
{
    public class MBank 
    {
        IBrowserHook _browser;

        public async Task StartScrapping(IBrowserHook browser)
        {
            _browser = browser
                ?? throw new ArgumentException();
            await _browser.Initialize();
            await Login();
            await SwitchToPrivateProfile();
            await GoToAccountsPage();
            await Logout();
        }


        private async Task Login()
        {
            await _browser.NavigateTo("https://online.mbank.pl/pl/Login");
            await _browser.WaitForPage("https://online.mbank.pl/dashboard");
            await _browser.WaitForElement("//span[text()='Miejsce na Twój produkt']");
        }

        private async Task SwitchToPrivateProfile()
        {
            if (await _browser.IsElementPresent("//div[text()='Profil indywidualny']"))
                return;

            await _browser.Click("//button[@data-test-id='app:profilesTrigger']");
            await _browser.Click("//span[contains(text(), 'wszystkie produkty')]/..");
            await _browser.WaitForElement("//div[text()='Profil indywidualny']");
        }

        private async Task SwitchToCompanyProfile()
        {
            if (await _browser.IsElementPresent("//div[text()='Profil firmowy']"))
                return;

            await _browser.Click("//button[@data-test-id='app:profilesTrigger']");
            await _browser.Click("//div[contains(text(), 'STAROSIELEC - SYSTEMY INFORMATYCZNE')]/..");
            await _browser.WaitForElement("//div[text()='Profil firmowy']");
        }

        private async Task Logout()
        {
            await _browser.Click("//a[@data-test-id='logout:link']");
            await _browser.WaitForPage(new Regex("https://www.mbank.pl/logoutpage.*"));
        }

        private async Task GoToAccountsPage()
        {
            await _browser.NavigateTo("https://online.mbank.pl/accounts2");
            await _browser.WaitForPage("https://online.mbank.pl/accounts2");
        }
    }
}
