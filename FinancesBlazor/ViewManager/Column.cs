using FinancesBlazor.DataAccess;

namespace FinancesBlazor.ViewManager;

public class Column
{
    public string Name { get; set; }

    public string Title { get; }

    public string Data { get; }

    public DataTypes DataType { get; }

    public string NullValue { get; }

    public string? Format { get; }

    public Align? Align { get; }

    public Column(string name, string title, string data, DataTypes dataType, string nullValue = "", string? format = null, Align? align = null)
    {
        Name = name;
        Title = title;
        Data = data;    
        DataType = dataType;
        NullValue = nullValue;
        Format = format;
        Align = align;
    }
}
