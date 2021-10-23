using System.Threading.Tasks;

namespace MBankScrapper
{
    public class MBank 
    {
        public async Task Login(IBrowserHook browser)
        {
            await browser.NavigateTo("https://online.mbank.pl/pl/Login");
            await browser.WaitForPage("https://online.mbank.pl/dashboard");
            await browser.WaitForElement("//span[text()='Miejsce na Twój produkt']");
        }

        public async Task SwitchToPrivateProfile(IBrowserHook browser)
        {
            if (await browser.IsElementPresent("//div[text()='Profil firmowy']"))
            {
                await browser.Click("//button[@data-test-id='app:profilesTrigger']");
            }
        
        }
    }
}
