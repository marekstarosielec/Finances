using System.Threading.Tasks;

namespace MBankScrapper
{
    public interface IBrowserHook
    {
        Task NavigateTo(string url);

        Task WaitForPage(string url);

        Task WaitForElement(string xpath);

        Task<bool> IsElementPresent(string xpath);

        Task Click(string xpath);
    }
}
