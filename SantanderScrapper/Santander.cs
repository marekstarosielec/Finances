using BrowserHook;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SantanderScrapper
{
    public class Santander
    {
        private IBrowserHook _browser;
        private ActionSet _actionSet;

        public async Task StartScrapping(IBrowserHook browser, ActionSet actionSet)
        {
            try
            {
                _browser = browser
                    ?? throw new ArgumentException();
                _actionSet = actionSet;

                await _browser.Initialize();
                await Login();
                await ScrapAccounts();
                await GoToTransactionsPage();
                await ScrapTransactions();
                await Logout();
            }
            catch (Exception e)
            {

            }
            finally
            {
                //     Process.GetProcesses().Where(p => string.Equals(p.ProcessName, "chromium", StringComparison.InvariantCultureIgnoreCase)).ToList().ForEach(proc => proc.Kill());
            }
        }

        private async Task Login()
        {
            await _browser.NavigateTo("https://www.centrum24.pl/centrum24-web");
            await _browser.WaitForPage(new Regex("https://www.centrum24.pl/centrum24-web/multi/dashboard*"));
            await _browser.WaitForElement("//div[@class='md-account-box']");
        }

        private async Task ScrapAccounts()
        {
            var title = await _browser.GetInnerText("//div[@class='md-account-name-number-section']/div[@class='md-account-top-data-line']");
            var iban = await _browser.GetInnerText("//div[@class='md-account-name-number-section']/span[@class='md-account-bottom-data-line']");
            var amount = await _browser.GetInnerText("//div[@class='md-account-amount-default-box']/div[@class='md-account-top-data-line']");
            
            if (title.Trim() == "Konto24 walutowe w EUR")
                title = "Santander EUR";

            decimal parsedAmount = 0;
            var currency = string.Empty;
            if (amount.LastIndexOf(" ") != -1)
            {
                currency = amount.Substring(amount.LastIndexOf(" ")).Trim();
                amount = amount.Replace(currency, string.Empty).Replace(" ", "").Replace(new string(new char[] { (char)160 }), string.Empty).Trim();
                _ = decimal.TryParse(amount, out parsedAmount);
            }

            var model = new Models.AccountBalance
            {
                Title = title,
                Iban = iban,
                Balance = parsedAmount,
                Currency = currency
            };
            _actionSet?.AccountBalance?.Invoke(model);
        }

        private async Task GoToTransactionsPage()
        {
            await _browser.NavigateTo("https://www.centrum24.pl/centrum24-web/multi/history");
            await _browser.WaitForPage(new Regex("https://www.centrum24.pl/centrum24-web/multi/history*"));
            await _browser.WaitForElement("//table[@id='transaction-table']/tbody/tr");
        }

        private async Task ScrapTransactions()
        {
            var transactionIndex = 1;
            while (await _browser.IsElementPresent($"//table[@id='transaction-table']/tbody/tr[{transactionIndex}]"))
            {
                var date = await _browser.GetInnerText($"//table[@id='transaction-table']/tbody/tr[{transactionIndex}]/td[@class='date']");
                var title = await _browser.GetInnerText($"//table[@id='transaction-table']/tbody/tr[{transactionIndex}]/td[@class='name']");
                var amount = await _browser.GetInnerText($"//table[@id='transaction-table']/tbody/tr[{transactionIndex}]/td[contains(@class,'amount')]");

                decimal parsedAmount = 0;
                var currency = string.Empty;
                if (amount.LastIndexOf(" ") != -1)
                {
                    currency = amount.Substring(amount.LastIndexOf(" ")).Trim();
                    amount = amount.Replace(currency, string.Empty).Replace(" ", "").Replace(new string(new char[] { (char)160 }), string.Empty).Trim();
                    _ = decimal.TryParse(amount, out parsedAmount);
                }

                DateTime.TryParseExact(date, "yyyy'-'MM'-'dd", null, System.Globalization.DateTimeStyles.AssumeUniversal, out var parsedDate);

                var transactionModel = new Models.Transaction
                {
                    Account = "Santander EUR",
                    Date = parsedDate,
                    Title = title,
                    Amount = parsedAmount,
                    Id = CreateMD5Hash($"{date}|{title}|{amount}"),
                    Currency = currency
                };
                _actionSet?.Transaction?.Invoke(transactionModel);

                transactionIndex = transactionIndex + 2; //There is a hidden row after every row.
            }
        }

        private async Task Logout()
        {
            await _browser.Click("//div[@class='logout']/a");
            await _browser.WaitForPage(new Regex("https://www.centrum24.pl/centrum24-web/logout"));
            await _browser.WaitForElement("//div[@class='logout-head']");
        }

        private string CreateMD5Hash(string input)
        {
            // Step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
