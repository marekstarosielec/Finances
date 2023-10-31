using Finances.DependencyInjection;
using FinancesBlazor.Components.Grid.Filters;

namespace FinancesBlazor.PropertyInfo;

public enum TextFilterComponents
{
    Default = 0
}

public partial class FilterComponentFactory : IInjectAsSingleton
{
    public Type? GetTextFilterComponent(TextFilterComponents? filterComponents) => filterComponents switch
    {
        null => null,
        TextFilterComponents.Default => typeof(TextFilter),
        _ => throw new ArgumentOutOfRangeException("Cannot find text filter component")
    };
}
