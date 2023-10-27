namespace FinancesBlazor.ViewManager;

public interface IEntity
{
    View GetView(IConfiguration configuration);
}
