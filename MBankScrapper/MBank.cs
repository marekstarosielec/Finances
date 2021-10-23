using System.Threading.Tasks;

namespace MBankScrapper
{
    public class MBank 
    {
        public async Task NavigateToLoginPage(IBrowserHook browser)
        {
            await browser.NavigateTo("https://online.mbank.pl/pl/Login");
            await browser.WaitForPage("https://online.mbank.pl/dashboard");
            await browser.WaitForElement("//span[text()='Miejsce na Twój produkt']");
        }
    }
}
