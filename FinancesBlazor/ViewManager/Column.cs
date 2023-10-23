using FinancesBlazor.DataAccess;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor.Rendering;

namespace FinancesBlazor.ViewManager;

public class Column
{
    public string Title { get; }

    public string Data { get; }

    public DataTypes DataType { get; }

    public string NullValue { get; }

    public string? Format { get; }

    public Align? Align { get; }

    public Column(string title, string data, DataTypes dataType, string nullValue = "", string? format = null, Align? align = null)
    {
        Title = title;
        Data = data;    
        DataType = dataType;
        NullValue = nullValue;
        Format = format;
        Align = align;
    }
}
