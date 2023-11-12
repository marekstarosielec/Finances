using DataSource;

namespace DataView;

public abstract class DataViewColumn
{
    public string PrimaryDataColumnName { get; }

    public string? SecondaryDataColumnName { get; }
    
    public DataViewColumnDataType DataType { get; }

    public string Title { get; }
    
    public string ShortName { get; set; }

    public string? NullValue { get; set; }

    public string? Format { get; set; }

    public DataViewColumnContentAlign? HorizontalAlign { get; set; }

    public Type? FilterComponentType { get; set; }

    public DataViewColumn(string primaryDataColumnName, DataViewColumnDataType dataType, string title, string shortName/*, Type? filterComponentType*/)
    {
        PrimaryDataColumnName = primaryDataColumnName;
        DataType = dataType;
        Title = title;
        ShortName = shortName;
        //FilterComponentType = filterComponentType;
    }
}
