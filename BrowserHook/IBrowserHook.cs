using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrowserHook
{
    public interface IBrowserHook
    {
        Task Initialize();

        Task NavigateTo(string url);

        Task WaitForPage(string url);

        Task WaitForPage(Regex regex);

        Task WaitForElement(string xpath);

        Task<bool> IsElementPresent(string xpath);

        Task<string> GetInnerText(string xpath);

        Task Click(string xpath);

        Task SetText(string xpath, string text);

        Task SendKey(string key, int count = 1);
    }
}
