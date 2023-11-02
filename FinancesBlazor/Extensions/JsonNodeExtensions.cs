using FinancesBlazor.PropertyInfo;
using System.Text.Json.Nodes;

namespace FinancesBlazor.Extensions;

public static class JsonNodeExtensions
{
    public static JsonNode? GetDeepNode(this JsonNode? value, PropertyInfoBase property)
    {
        var result = value;
        var subProperties = property.PropertyName.Split('.');
        for (int i = 0; i < subProperties.Length - 1; i++)
            if (result != null)
                result = result[subProperties[i]];
        return result;
    }
}
