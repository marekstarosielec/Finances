﻿using BrowserHook;
using System;
using System.Collections.Generic;
using System.Linq;
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
            await GoToSavingsPage();
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

        private async Task GoToSavingsPage()
        {
            await _browser.NavigateTo("https://online.mbank.pl/savings2");
            await _browser.WaitForPage("https://online.mbank.pl/savings2");
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
                if (string.IsNullOrWhiteSpace(title)) //Savings have additional icon in first div
                {
                    titleXPath = $"{accountXPath(accountIndex)}/div[2]/div[2]/div[1]";
                    title = await _browser.GetInnerText(titleXPath);
                }
                title = title
                    .Replace("eKonto - ", "")
                    .Replace("eKonto walutowe EUR - ", "")
                    .Replace("eMax - ", "")
                    .Replace("mBiznes konto - ", "")
                    .Replace(" - Konto VAT", "");

                var iban = "";
                if (await _browser.IsElementPresent($"{accountXPath(accountIndex)}/div[2]/div[2]/div/div/button/span[2]"))
                {
                    iban = await _browser.GetInnerText($"{accountXPath(accountIndex)}/div[2]/div[2]/div/div/button/span[2]");
                    iban = iban.Replace(" ", "");
                }

                var balance = "0";
                if (await _browser.IsElementPresent($"{accountXPath(accountIndex)}/div[2]/div[3]/span"))
                    balance = await _browser.GetInnerText($"{accountXPath(accountIndex)}/div[2]/div[3]/span");
                else if(await _browser.IsElementPresent($"{accountXPath(accountIndex)}/div[2]/div[4]"))
                    balance = await _browser.GetInnerText($"{accountXPath(accountIndex)}/div[2]/div[4]");
                
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
            //var spinner = "//div[@class='_PMtueryzQy6EvzsZf3o']";
            await _browser.Click(allButton);
            await _browser.Click(allButton);
            var accountNumber = 2;
            while (await _browser.IsElementPresent(GetAccountFilterXPath(accountNumber)))
            {
                await _browser.Click(GetAccountFilterXPath(accountNumber));
                var title = await _browser.GetInnerText(GetAccountFilterXPath(accountNumber));
                title = title
                    .Replace("\n", "")
                    .Replace("EUR eKonto walutowe", "")
                    .Replace("eKonto", "")
                    .Replace("eMax", "");
                //.Replace("mBiznes konto - ", "")
                //.Replace(" - Konto VAT", "");
                var a = _accounts.FirstOrDefault(ai => ai.Title == title);
                if (a == null)
                {
                    //Filtering option is not on a list of accounts (from accounts and savings screen).
                    //Propably some bank specific filter so ignoring it.
                    await _browser.Click(GetAccountFilterXPath(accountNumber));
                    accountNumber++;
                    continue;
                }
                await SetDateFilter(DateTime.Today.AddYears(-1).AddDays(1), DateTime.Today);
                //var spinnerPresent = await _browser.IsElementPresent(spinner); 
                var transactionCount = await TransactionCount();
                if (transactionCount.total > 50)
                {
                    //There is a lot of transaction in the view. To avoid slowing page down,
                    //navigate thru transactions day by day.
                    var currentDay = DateTime.Now;
                    var daysWithoutNoNewTransactions = 0;
                    while (daysWithoutNoNewTransactions < 7)
                    {
                        await SetDateFilter(currentDay, currentDay);
                        transactionCount = await TransactionCount();
                        var newTransactions = await ScrapVisibleTransactions();
                        if (newTransactions == 0)
                            daysWithoutNoNewTransactions++;

                        currentDay = currentDay.AddDays(-1);
                    }
                }
                else
                {
                    await ScrapVisibleTransactions();
                }
                //Thread.Sleep(2000);

                await _browser.Click(GetAccountFilterXPath(accountNumber));
                accountNumber++;
            };

            async Task ShowAllAccounts()
            {
                var moreButton = "//button[@data-test-id='history:showMoreProductsFilter']/span[text()='więcej']";
                if (await _browser.IsElementPresent(moreButton))
                    await _browser.Click(moreButton);
            }

            async Task<(int loaded, int total)> TransactionCount()
            {
                var counter = await _browser.GetInnerText("//div[@testid='OperationsSummary:operationsCount']");
                //25 z 4042 operacji
                (int loaded, int total) = (-1, -1);
                foreach (var c in counter.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    if (int.TryParse(c, out int parsedCounter))
                        if (loaded == -1) 
                            loaded = parsedCounter;
                        else 
                            total = parsedCounter;
                return (loaded, total);
            }
            
            async Task<int> ScrapVisibleTransactions()
            {
                await ScrollToLoadAllTransactionsInFilter();
                var transactionCount = await TransactionCount();
                for (var currentTransaction = 0; currentTransaction < transactionCount.total; currentTransaction++)
                {
                    var tags = "";
                    if (await _browser.IsElementPresent($"{GetTransactionRow(currentTransaction)}/descendant::span[@data-test-id='Tooltip:tags-trigger']"))
                        tags = await _browser.GetInnerText($"{GetTransactionRow(currentTransaction)}/descendant::span[@data-test-id='Tooltip:tags-trigger']");
                    if (tags == "scrapped")
                        continue;

                    var type = await _browser.GetInnerText($"{GetTransactionRow(currentTransaction)}/descendant::span[@data-test-id='Tooltip:operationType-trigger']");
                    var date = await _browser.GetInnerText($"{GetTransactionRow(currentTransaction)}/descendant::span[4]");
                    var title = await _browser.GetInnerText($"{GetTransactionRow(currentTransaction)}/td[3]");
                    var amount = await _browser.GetInnerText($"{GetTransactionRow(currentTransaction)}/td[position()=6]");
                    var comment = "";
                    if (await _browser.IsElementPresent($"{GetTransactionRow(currentTransaction)}/descendant::span[@data-test-id='Tooltip:comment-trigger']"))
                        comment = await _browser.GetInnerText($"{GetTransactionRow(currentTransaction)}/descendant::span[@data-test-id='Tooltip:comment-trigger']");
                    
                }

                return 0;
            }

            string GetTransactionRow(int transactionNumber) => 
                $"//tr[@data-test-id='history:operationRow:{transactionNumber}']"; 

            async Task ScrollToLoadAllTransactionsInFilter()
            {
                await _browser.Click("//div[@testid='OperationsSummary:operationsCount']");
                var transactionCount = await TransactionCount();
                while (transactionCount.loaded != transactionCount.total)
                {
                    await _browser.SendKey("Home");
                    await _browser.SendKey("End");
                    transactionCount = await TransactionCount();
                }
            }

            async Task SetDateFilter(DateTime from, DateTime to)
            {
                var dateFromXPath = "//html/descendant::input[@class='DateInput_input DateInput_input_1'][1]";
                var dateToXPath = "//html/descendant::input[@class='DateInput_input DateInput_input_1'][2]";

                await _browser.SetText(dateFromXPath, from.ToString("dd'.'MM'.'yyyy"));
                await _browser.SendKey("Delete", 10);
                await _browser.SetText(dateToXPath, to.ToString("dd'.'MM'.'yyyy"));
                await _browser.SendKey("Delete", 10);
            }
            string GetAccountFilterXPath(int accountNumber) => $"//span[text()='wszystkie']//ancestor::ul/parent::*/descendant::li[{accountNumber}]";
        }
    }
    ////div[@class='_PMtueryzQy6EvzsZf3o'] - spinner on transactions
}
