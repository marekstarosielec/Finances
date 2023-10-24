using Finances.DependencyInjection;
using System.Web;

namespace FinancesBlazor.ViewManager;

public class ViewListParametersSerializer : IInjectAsSingleton
{
    public async Task<string> Serialize(string activeViewName, ViewListParameters? parameters)
    {
        if (parameters == null)
            return string.Empty;

        var data = new Dictionary<string, string>();
        data["av"] = activeViewName;
        if (!string.IsNullOrWhiteSpace(parameters.SortingColumnDataName))
            data["s"] = parameters.SortingColumnDataName;
        if (parameters.SortingDescending)
            data["d"] = "1";
        foreach (var filter in parameters.Filters)
        {
            if (!string.IsNullOrWhiteSpace(filter.Value.StringValue))
                data[$"f_{filter.Key.Name}_sv"] = filter.Value.StringValue;
        }
        return await new FormUrlEncodedContent(data).ReadAsStringAsync();
    }

    public DeserializationResult Deserialize(string serializedValue)
    {
        var result = new DeserializationResult();
        var nvc = HttpUtility.ParseQueryString(serializedValue);
        var items = nvc.AllKeys.SelectMany(nvc.GetValues, (k, v) => new { key = k, value = v });
        foreach (var item in items)
        {
            if (item.key == "av")
                result.ActiveViewName = item.value;
            else if (item.key == "s")
                result.SortingColumnDataName = item.value;
            else if (item.key == "d" && item.value=="1")
                result.SortingDescending = true;
            else if (item.key?.StartsWith("f_") == true && item.key?.EndsWith("_sv") == true)
            {
                var name = item.key[2..^3];
                result.Filters[name] = new FilterValue {  StringValue = item.value };
            }
        }

        return result;
    }
}


public class DeserializationResult
{
    public string? ActiveViewName { get; set; }

    public string? SortingColumnDataName { get; set; }

    public bool SortingDescending { get; set; }

    public Dictionary<string, FilterValue> Filters { get; set; } = new Dictionary<string, FilterValue>();
}