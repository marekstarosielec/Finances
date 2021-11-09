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
        ActionSet _actionSet;

        public async Task StartScrapping(IBrowserHook browser, ActionSet actionSet)
        {
            _browser = browser
                ?? throw new ArgumentException();
            _actionSet = actionSet;

            await _browser.Initialize();
            await Login();
            await SwitchToPrivateProfile();
            if (_actionSet?.AccountBalance != null)
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

                _actionSet?.AccountBalance?.Invoke(new Models.AccountBalance
                {
                    Title = title,
                    Iban = iban,
                    Balance = parsedBalance,
                    Currency = currency
                });

                accountIndex++;
            }
            //var groups = _browser.("//li[@data-ccl='true'][0]");
            //foreach (var group in groups)
            //{
            //        var elements = _browser.GetElements(group.XPath + Elements.GetBalanceList());
            //        foreach (var element in elements)
            //        {
            //            var bankAccount = new BankAccount();
            //            bankAccount.Title = _browser.GetElement(element.XPath + Elements.GetBalanceAccountTitle()).GetText().Replace("eKonto - ", "").Replace("eKonto walutowe EUR - ", "").Replace("eMax - ", "").Replace("mBiznes konto - ", "").Replace(" - Konto VAT", "");
            //            bankAccount.Number = _browser.GetElement(element.XPath + Elements.GetBalanceAccountNumber()).GetText().Replace("Skopiuj numer rachunku", "").Replace(" ", "");
            //            bankAccount.Amount = _browser.GetElement(element.XPath + Elements.GetBalanceAmount()).GetText();
            //            bankAccount.Currency = bankAccount.Amount.GetCurrency();
            //            bankAccount.Amount = bankAccount.Amount.GetAmountWithoutCurrency(bankAccount.Currency);
            //            result.Add(bankAccount);
            //        }

            //        elements = _browser.GetElements(group.XPath + Elements.GetSavingList());
            //        foreach (var element in elements)
            //        {
            //            var bankAccount = new BankAccount();
            //            bankAccount.Title = _browser.GetElement(element.XPath + Elements.GetBalanceSavingTitle()).GetText();
            //            bankAccount.Number = "0";
            //            bankAccount.Amount = _browser.GetElement(element.XPath + Elements.GetBalanceSavingAmount()).GetText();
            //            bankAccount.Currency = bankAccount.Amount.GetCurrency();
            //            bankAccount.Amount = bankAccount.Amount.GetAmountWithoutCurrency(bankAccount.Currency);
            //            result.Add(bankAccount);
            //        }
            //    }



            //    return result;
            //}

            string accountXPath(int accountIndex) => $"//html/descendant::li[{accountIndex}]";
        }
    }
}
