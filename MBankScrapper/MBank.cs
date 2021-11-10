using BrowserHook;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MBankScrapper
{
    public class MBank 
    {
        IBrowserHook _browser;
        ActionSet _actionSet;
        List<Models.AccountBalance> _accounts = new ();

        public async Task StartScrapping(IBrowserHook browser, ActionSet actionSet)
        {
            _browser = browser
                ?? throw new ArgumentException();
            _actionSet = actionSet;

            await _browser.Initialize();
            await Login();
            await SwitchToPrivateProfile();
            await GoToAccountsPage();
            await ScrapAccounts();
            await GoToTransactionsPage();
            await ScrapTransactions();
            Thread.Sleep(5000);
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

        private async Task ScrapAccounts()
        {
            var accountIndex = 1;
            while (await _browser.IsElementPresent(accountXPath(accountIndex)))
            {
                var titleXPath = $"{accountXPath(accountIndex)}/div[2]/div[1]/div[1]";
                if (!await _browser.IsElementPresent(titleXPath))
                {
                    accountIndex++;
                    continue;
                }
                var title = await _browser.GetInnerText(titleXPath);
                title = title
                    .Replace("eKonto - ", "")
                    .Replace("eKonto walutowe EUR - ", "")
                    .Replace("eMax - ", "")
                    .Replace("mBiznes konto - ", "")
                    .Replace(" - Konto VAT", "");

                var iban = await _browser.GetInnerText($"{accountXPath(accountIndex)}/div[2]/div[2]/div/div/button/span[2]");
                iban = iban.Replace(" ", "");

                var balance = await _browser.GetInnerText($"{accountXPath(accountIndex)}/div[2]/div[3]/span");
                decimal parsedBalance = 0;
                var currency = string.Empty;
                if (balance.LastIndexOf(" ") != -1)
                {
                    currency = balance.Substring(balance.LastIndexOf(" ")).Trim();

                    char[] whiteSpaces = { (char)160 };
                    balance = balance.Replace(currency, string.Empty).Replace(" ", "").Replace(new string(whiteSpaces), string.Empty).Trim();
                    decimal.TryParse(balance, out parsedBalance);
                }

                var model = new Models.AccountBalance
                {
                    Title = title,
                    Iban = iban,
                    Balance = parsedBalance,
                    Currency = currency
                };
                _accounts.Add(model);
                _actionSet?.AccountBalance?.Invoke(model);

                accountIndex++;
            }
            string accountXPath(int accountIndex) => $"//html/descendant::li[{accountIndex}]";
        }

        private async Task GoToTransactionsPage()
        {
            await _browser.NavigateTo("https://online.mbank.pl/history");
            await _browser.WaitForPage(new Regex("https://online.mbank.pl/history.*"));
        }

        private async Task ScrapTransactions()
        {
            await ShowAllAccounts();
            var allButton = "//span[text()='wszystkie']//ancestor::li";
            await _browser.Click(allButton);
            await _browser.Click(allButton);
            foreach(var account in _accounts)
            {
                await FilterBy(account.Title);
            };

            async Task ShowAllAccounts()
            {
                var moreButton = "//button[@data-test-id='history:showMoreProductsFilter']/span[text()='więcej']";
                if (await _browser.IsElementPresent(moreButton))
                    await _browser.Click(moreButton);
            }

            async Task FilterBy(string accountName)
            {
                try
                {
                    await _browser.Check($"//div[@title='{accountName}']//ancestor::li/descendant::input");
                    Thread.Sleep(2000);
                }
                catch(Exception e)
                {

                }
            }
        }
    }
    ////div[@class='_PMtueryzQy6EvzsZf3o'] - spinner on transactions
}
