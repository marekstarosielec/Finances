using Finances.DependencyInjection;

namespace FinancesBlazor.ViewManager;

public partial class ViewsList : IInjectAsSingleton
{
    private static IConfiguration? _configuration;

    public ViewsList(IConfiguration configuration)
    {
        _configuration = configuration;
    }
}
