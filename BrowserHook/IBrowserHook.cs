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

        Task Click(string xpath);
    }
}
