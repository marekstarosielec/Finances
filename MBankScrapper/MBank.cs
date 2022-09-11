using BrowserHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        const int sleepTime = 1500;

        public async Task StartScrapping(IBrowserHook browser, ActionSet actionSet)
        {
            try
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
                await GoToCardsPage();
                await ScrapCards();
                await GoToTransactionsPage();
                await ScrapTransactions();
                await SwitchToCompanyProfile();
                await GoToAccountsPage();
                await ScrapAccounts();
                await GoToTransactionsPage();
                await ScrapTransactions();
                await SwitchToPrivateProfile();
                await Logout();
            }
            catch(Exception e) 
            {

            }
            finally
            {
           //     Process.GetProcesses().Where(p => string.Equals(p.ProcessName, "chromium", StringComparison.InvariantCultureIgnoreCase)).ToList().ForEach(proc => proc.Kill());
            }
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
            await _browser.WaitForPage(new Regex("https://www.mbank.pl/logoutpage.*"), new Regex("https://online.mbank.pl/pl/Login"), new Regex("https://online.mbank.pl/errors/session.html"));
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

        private async Task GoToCardsPage()
        {
            await _browser.NavigateTo("https://online.mbank.pl/cards2");
            await _browser.WaitForPage("https://online.mbank.pl/cards2");
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
                    .Replace("mBiznes konto walutowe EUR - ", "")
                    .Replace("eKonto walutowe EUR - ", "")
                    .Replace("eKonto walutowe GBP - ", "")
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
                    balance = balance.Replace(currency, string.Empty).Replace(" ", "").Replace(new string(new char[] { (char)160 }), string.Empty).Trim();
                    _ = decimal.TryParse(balance, out parsedBalance);
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

        private async Task ScrapCards()
        {
            var accountIndex = 1;
            while (await _browser.IsElementPresent(accountXPath(accountIndex)))
            {
                var titleXPath = $"{accountXPath(accountIndex)}/descendant::p";
                var amountXPath = $"{accountXPath(accountIndex)}/descendant::span[3]";

                if (!await _browser.IsElementPresent(titleXPath))
                {
                    accountIndex++;
                    continue;
                }
                var title = await _browser.GetInnerText(titleXPath);
                if (title == "VISA PAYWAVE" || title == "MASTERCARD PAYPASS")
                {
                    accountIndex++;
                    continue;
                }
                title = title.Replace("Do aktywacji", string.Empty).Trim();
                var balance = "0";
                if (await _browser.IsElementPresent(amountXPath))
                    balance = await _browser.GetInnerText(amountXPath);
               
                decimal parsedBalance = 0;
                var currency = string.Empty;
                if (balance.LastIndexOf(" ") != -1)
                {
                    currency = balance.Substring(balance.LastIndexOf(" ")).Trim();

                    char[] whiteSpaces = { (char)160 };
                    balance = balance.Replace(currency, string.Empty).Replace(" ", "").Replace(new string(whiteSpaces), string.Empty).Trim();
                    _ = decimal.TryParse(balance, out parsedBalance);
                }
                if (parsedBalance == 0)
                {
                    var amountXPath2 = $"{accountXPath(accountIndex)}/descendant::span[4]";
                    if (await _browser.IsElementPresent(amountXPath2))
                        balance = await _browser.GetInnerText(amountXPath2);

                    if (balance.LastIndexOf(" ") != -1)
                    {
                        currency = balance.Substring(balance.LastIndexOf(" ")).Trim();

                        char[] whiteSpaces = { (char)160 };
                        balance = balance.Replace(currency, string.Empty).Replace(" ", "").Replace(new string(whiteSpaces), string.Empty).Trim();
                        _ = decimal.TryParse(balance, out parsedBalance);
                    }
                }

                var model = new Models.AccountBalance
                {
                    Title = title,
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
            var noTransactionXPath = "//span[text()='Brak operacji dla wybranych kryteriów wyszukiwania']";
            await _browser.Click(allButton);
            if (!await _browser.IsElementPresent(noTransactionXPath))
                await _browser.Click(allButton);
            var accountNumber = 2;
            while (await _browser.IsElementPresent(GetAccountFilterXPath(accountNumber)))
            {
                await _browser.Click(GetAccountFilterXPath(accountNumber));
                var title = await _browser.GetInnerText(GetAccountFilterXPath(accountNumber));
                title = title
                    .Replace("EUR mBiznes konto walutowe", "")
                    .Replace("EUR eKonto walutowe", "")
                    .Replace("GBP eKonto walutowe", "")
                    .Replace("eKonto", "")
                    .Replace("eMax", "")
                    .Replace("mBiznes konto", "")
                    .Replace("\nKonto VAT", "")
                    .Replace("\n", "")
                    .Replace(" - ", "");

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
                var transactionCount = await TransactionCount();
                if (transactionCount.total > 50)
                {
                    //There is a lot of transaction in the view. To avoid slowing page down,
                    //navigate thru transactions day by day.
                    var currentDay = DateTime.Now;
                    var daysWithoutNewTransactions = 0;
                    while (daysWithoutNewTransactions < 7)
                    {
                        await SetDateFilter(currentDay, currentDay);
                        var newTransactions = await ScrapVisibleTransactions(a.Title);
                        if (newTransactions == 0)
                            daysWithoutNewTransactions++;
                        else
                            daysWithoutNewTransactions = 0;

                        currentDay = currentDay.AddDays(-1);
                    }
                }
                else
                {
                    await ScrapVisibleTransactions(a.Title);
                }

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
                var noTransactionXPath = "//span[text()='Brak operacji dla wybranych kryteriów wyszukiwania']";
                if (await _browser.IsElementPresent(noTransactionXPath))
                    return (0,0);

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
            
            async Task<int> ScrapVisibleTransactions(string account)
            {
                var noTransactionXPath = "//span[text()='Brak operacji dla wybranych kryteriów wyszukiwania']";
                if (await _browser.IsElementPresent(noTransactionXPath))
                    return 0;
                await ScrollToLoadAllTransactionsInFilter();
                var transactionCount = await TransactionCount();
                var result = 0;
                for (var currentTransaction = 0; currentTransaction < transactionCount.total; currentTransaction++)
                {
                    var tags = "";
                    if (await _browser.IsElementPresent($"{GetTransactionRow(currentTransaction)}/descendant::span[@data-test-id='Tooltip:tags-trigger']"))
                        tags = await _browser.GetInnerText($"{GetTransactionRow(currentTransaction)}/descendant::span[@data-test-id='Tooltip:tags-trigger']");
                    if (tags == "scrapped")
                        continue;

                    var type = await _browser.GetInnerText($"{GetTransactionRow(currentTransaction)}/descendant::span[@data-test-id='Tooltip:operationType-trigger']");
                    var date = await _browser.GetInnerText($"{GetTransactionRow(currentTransaction)}/descendant::span[4]");
                    var text = await _browser.GetInnerText($"{GetTransactionRow(currentTransaction)}/td[3]");
                    var amount = await _browser.GetInnerText($"{GetTransactionRow(currentTransaction)}/td[position()=6]");
                    var comment = "";
                    if (await _browser.IsElementPresent($"{GetTransactionRow(currentTransaction)}/descendant::span[@data-test-id='Tooltip:comment-trigger']"))
                        comment = await _browser.GetInnerText($"{GetTransactionRow(currentTransaction)}/descendant::span[@data-test-id='Tooltip:comment-trigger']");

                    var id = Guid.NewGuid().ToString();
                    var pos = comment.IndexOf("scrapid:", StringComparison.InvariantCultureIgnoreCase);
                    if (pos > -1 && string.Equals(type, "nierozliczone", StringComparison.InvariantCultureIgnoreCase))
                        continue; //If id is added and type is "nierozliczone" means it was scrapped already and not yet settled.

                    if (pos > -1)
                        id = comment.Substring(pos + 8, 36); //get id from comment

                    decimal parsedAmount = 0;
                    amount = amount.Replace((char)8201, ' ');
                    var currency = string.Empty;
                    if (amount.LastIndexOf(" ") != -1)
                    {
                        currency = amount.Substring(amount.LastIndexOf(" ")).Trim();

                        char[] whiteSpaces = { (char)160 };
                        amount = amount.Replace(currency, string.Empty).Replace(" ", "").Replace(new string(whiteSpaces), string.Empty).Trim();
                        _ = decimal.TryParse(amount, out parsedAmount);
                    }

                    await ExpandTransactionRow(currentTransaction);
                    var titleXPath = $"{GetTransactionRow(currentTransaction)}/following::tr/descendant::div[@data-test-id='GenericDetails:Title:0']";
                    var title = string.Empty;
                    if (await _browser.IsElementPresent(titleXPath))
                        title = await _browser.GetInnerText(titleXPath);

                    var descriptionXPath = $"{GetTransactionRow(currentTransaction)}/following::tr/descendant::div[@data-test-id='GenericDetails:TransferDescription:0']";
                    var description = string.Empty;
                    if (await _browser.IsElementPresent(descriptionXPath))
                        description = await _browser.GetInnerText(descriptionXPath);

                    await EditTransaction(currentTransaction, comment);
                    if (pos == -1)
                        await SetComment($"scrapid:{id}");
                    if (!string.Equals(type, "nierozliczone", StringComparison.InvariantCultureIgnoreCase))
                        await SetScrappedTag();
                    await SaveTransaction();

                    var transactionModel = new Models.Transaction
                    {
                        Date = date,
                        Text = text,
                        Title = title,
                        Description = description,
                        Account = account,
                        Amount = parsedAmount,
                        Id = id,
                        Currency = currency
                    };
                    _actionSet?.Transaction?.Invoke(transactionModel);
                    result++;
                }

                return result;
            }

            string GetTransactionRow(int transactionNumber) => 
                $"//tr[@data-test-id='history:operationRow:{transactionNumber}']";

            async Task ExpandTransactionRow(int transactionNumber)
            {
                while (!await _browser.IsElementPresent($"{GetTransactionRow(transactionNumber)}/following::tr[@aria-hidden='false']"))
                {
                    await _browser.Click($"{GetTransactionRow(transactionNumber)}");
                    Sleep();
                }
                    
            }

            async Task EditTransaction(int transactionNumber, string comment)
            {
                var editButtonXPath = $"{GetTransactionRow(transactionNumber)}/following::tr/descendant::button[@data-test-id='history:edit']";
                var commentInputXPath = $"{GetTransactionRow(transactionNumber)}/following::tr/descendant::input[@data-test-id='Comment:Input']";
                while (await _browser.IsElementPresent(editButtonXPath))
                {
                    await _browser.Click(editButtonXPath);
                    Sleep(); 
                }

                //MBank shows comment from previous transaction for a few milliseconds.
                //Need to make sure it is correct before continuing.

                if (string.IsNullOrWhiteSpace(comment))
                    //No comment in transaction - input should not be visible.
                    while (await _browser.IsElementPresent(commentInputXPath))
                        Sleep();
                else
                {
                    //Transaction commented. Wait for input and check if it has
                    //valid content.
                    while (!await _browser.IsElementPresent(commentInputXPath))
                        Sleep();

                    while (await _browser.GetInputValue(commentInputXPath) != comment)
                        Sleep();
                }
            }

            async Task ScrollToLoadAllTransactionsInFilter()
            {
                await _browser.Click("//div[@testid='OperationsSummary:operationsCount']");
                var transactionCount = await TransactionCount();
                while (transactionCount.loaded != transactionCount.total)
                {
                    //Sometime "End" alone is not enough - page is stuck with spinner.
                    //Call "Home" to to trigger loading.
                    await _browser.SendKey("Home");
                    await _browser.SendKey("End");
                    Sleep();
                    transactionCount = await TransactionCount();
                }
            }

            async Task SetDateFilter(DateTime from, DateTime to)
            {
                var dateFromXPath = "//html/descendant::input[@class='DateInput_input DateInput_input_1'][1]";
                var dateToXPath = "//html/descendant::input[@class='DateInput_input DateInput_input_1'][2]";
                var spinner = "//div[@class='_PMtueryzQy6EvzsZf3o']";
                var noTransactionXPath = "//span[text()='Brak operacji dla wybranych kryteriów wyszukiwania']";

                await _browser.SetText(dateFromXPath, from.ToString("dd'.'MM'.'yyyy"));
                await _browser.SendKey("Delete", 10);
                await _browser.SetText(dateToXPath, to.ToString("dd'.'MM'.'yyyy"));
                await _browser.SendKey("Delete", 10);

                await _browser.WaitForElement(spinner, continueOnTimeout: true, timeoutMilliseconds: 1000); //Sometimes spinner disappears before it can be checked for.
                while ((!await _browser.IsElementPresent(noTransactionXPath)) && (!await _browser.IsElementPresent(GetTransactionRow(0)))) { } //Wait for transactions to appear
            }

            async Task SetComment(string comment)
            {
                var deleteButtonXPath = "//a[@data-test-id='Comment:RemoveButton']";
                var addButtonXPath = "//button[@data-test-id='Comment:AddButton']";
                
                if (await _browser.IsElementPresent(deleteButtonXPath))
                    await _browser.Click(deleteButtonXPath);
                if (await _browser.IsElementPresent(addButtonXPath))
                    await _browser.Click(addButtonXPath);
                await _browser.SetText("//input[@data-test-id='Comment:Input']", comment);
            }

            async Task SetScrappedTag() => await _browser.Click("//button[text()='scrapped' and contains(@data-test-id,'SelectionSwitch:Tag')]");

            async Task SaveTransaction() =>
                await _browser.Click("//button[@data-test-id='EditDetails:SaveButton']");

            string GetAccountFilterXPath(int accountNumber) => $"//span[text()='wszystkie']//ancestor::ul/parent::*/descendant::li[{accountNumber}]";
        }

        void Sleep(int sleepLength = 0) => Thread.Sleep(sleepLength == 0 ? sleepTime: sleepLength);
    }
}
