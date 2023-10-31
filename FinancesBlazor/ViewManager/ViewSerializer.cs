using Finances.DependencyInjection;
using System.Web;

namespace FinancesBlazor.ViewManager;

public class ViewSerializer : IInjectAsSingleton
{
    public string Serialize(string activeViewName, View? view)
    {
        if (view == null)
            return string.Empty;

        var data = new Dictionary<string, string>();
        data["av"] = activeViewName;
        if (!string.IsNullOrWhiteSpace(view.SortingColumnPropertyName))
            data["s"] = view.SortingColumnPropertyName;
        if (view.SortingDescending)
            data["d"] = "1";
        foreach (var filter in view.Filters)
        {
            if (!string.IsNullOrWhiteSpace(filter.Value.StringValue))
                data[$"f_{filter.Key.ShortName}_sv"] = filter.Value.StringValue;
            if (filter.Value.DateFrom != null)
                data[$"f_{filter.Key.ShortName}_fr"] = filter.Value.DateFrom.Value.ToString("yyyyMMdd");
            if (filter.Value.DateTo != null)
                data[$"f_{filter.Key.ShortName}_to"] = filter.Value.DateTo.Value.ToString("yyyyMMdd");
        }
        return new StreamReader(new FormUrlEncodedContent(data).ReadAsStream()).ReadToEnd();
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
                result.Filters[name] = new FilterInfoBase {  StringValue = item.value };
            }
            else if (item.key?.StartsWith("f_") == true && item.key?.EndsWith("_fr") == true)
            {
                var name = item.key[2..^3];
                result.Filters[name] ??= new FilterInfoBase();
                result.Filters[name].DateFrom = DateTime.ParseExact(item.value, "yyyyMMdd", null);
            }
            else if (item.key?.StartsWith("f_") == true && item.key?.EndsWith("_to") == true)
            {
                var name = item.key[2..^3];
                result.Filters[name] ??= new FilterInfoBase();
                result.Filters[name].DateTo = DateTime.ParseExact(item.value, "yyyyMMdd", null);
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

    public Dictionary<string, FilterInfoBase> Filters { get; set; } = new Dictionary<string, FilterInfoBase>();
}