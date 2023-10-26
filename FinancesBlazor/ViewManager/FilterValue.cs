namespace FinancesBlazor.ViewManager;

public class FilterValue
{
    //Properties added here need to be included in ViewListParametersSerializer.
    public string? StringValue { get; set; }

    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }
}
