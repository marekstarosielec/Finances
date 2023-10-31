using Finances.DependencyInjection;
using FinancesBlazor.Components.Grid.Filters;

namespace FinancesBlazor.PropertyInfo;

public enum DateFilterComponents
{
    Default = 0
}

public partial class FilterComponentFactory : IInjectAsSingleton
{
    public Type? GetDateFilterComponent(DateFilterComponents? filterComponents) => filterComponents switch
    {
        null => null,
        DateFilterComponents.Default => typeof(DateFilter),
        _ => throw new ArgumentOutOfRangeException("Cannot find date filter component")
    };
}
