using Finances.DependencyInjection;

namespace FinancesBlazor.ViewManager;

public partial class ViewManager : IInjectAsSingleton
{
    private readonly ViewsList _viewsList;
    private View _activeView;

    public View ActiveView { get => _activeView; set => _activeView = value; }

    public ViewManager(ViewsList viewsList)
    {
        _viewsList = viewsList;
        _activeView = _viewsList.Electricity;
    }
}

