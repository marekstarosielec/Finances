namespace FinancesBlazor.DataTypes;

public class ComponentMetadata
{
    public Type Type { get; set; }
    public string? Name { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

    public ComponentMetadata(Type type)
    {
        Type = type;
    }

}
