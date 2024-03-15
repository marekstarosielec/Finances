namespace FinancesBlazor.Extensions;

using Microsoft.JSInterop;
using System.Threading.Tasks;

public static class JSRuntimeExtensions
{
    public async static Task ChangeRouteWithoutReload(this IJSRuntime js, string newRoute)
    {
        await js.InvokeAsync<string>("window.history.pushState", new { }, string.Empty, newRoute);
    }

    public async static Task<string> GetQueryString(this IJSRuntime js) => await js.InvokeAsync<string>("getQueryString");
}
