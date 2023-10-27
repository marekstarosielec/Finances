using Finances.DependencyInjection;
using FinancesBlazor.Components.Grid.Filters;

namespace FinancesBlazor.DataTypes;

public class DataTypesFilterComponents: IInjectAsSingleton
{
    private Dictionary<DataTypesList, ComponentMetadata> _filterComponents = new() {
    {
        DataTypesList.Text,
        new ComponentMetadata(typeof(TextFilter))
        {
            Name = "Rocket Lab with Window Seat",
            Parameters = new() { { "WindowSeat", false } }
        }
    },
    {
        DataTypesList.Date,
        new ComponentMetadata(typeof(DateFilter)) { Name = "ULA" }
    },
    {
        DataTypesList.Precision,
        new ComponentMetadata(typeof(PrecisionFilter)) { Name = "Virgin Galactic" }
    }};

    public ComponentMetadata GetFilterComponent(DataTypesList dataType) => _filterComponents.ContainsKey(dataType) ? _filterComponents[dataType] : throw new InvalidOperationException($"Filter component not defined for {dataType}");
}
