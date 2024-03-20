namespace DataViews;

public class DataViewColumn
{
    public string PrimaryDataColumnName { get; }

    public string? SecondaryDataColumnName { get; set; }
    
    public DataViewColumnDataType DataType { get; }

    public string Title { get; }
    
    public string ShortName { get; }

    public string? NullValue { get; set; }

    public string? Format { get; set; }

    public DataViewColumnContentAlign? HorizontalAlign { get; set; }

    public string? PreferredFilterComponentType { get; set; }
    public bool Visible { get; set; }
    public int? NumberOfLinesInDetails { get; set; }

    public string? ReferencedDataViewName { get; set; }

    public DataViewColumn(string primaryDataColumnName, DataViewColumnDataType dataType, string title, string shortName, string? preferredFilterComponentType = null, bool visible = true)
    {
        PrimaryDataColumnName = primaryDataColumnName;
        DataType = dataType;
        Title = title;
        ShortName = shortName;
        PreferredFilterComponentType = preferredFilterComponentType;
        Visible = visible;
    }

    public DataViewColumn Clone() => new DataViewColumn(PrimaryDataColumnName, DataType, Title, ShortName, PreferredFilterComponentType, Visible)
    {
        SecondaryDataColumnName = SecondaryDataColumnName,
        NullValue = NullValue,
        Format = Format,
        HorizontalAlign = HorizontalAlign,
        NumberOfLinesInDetails = NumberOfLinesInDetails,
        ReferencedDataViewName = ReferencedDataViewName,
    };
}
