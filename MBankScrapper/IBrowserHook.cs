using System.Threading.Tasks;

namespace MBankScrapper
{
    public interface IBrowserHook
    {
        Task NavigateTo(string url);

        Task WaitForPage(string url);

        Task WaitForElement(string xpath);
    }
}
