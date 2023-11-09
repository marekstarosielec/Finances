using DataSource;

namespace DataView;

public abstract class DataViewColumn
{
    public string DataColumnName { get; }
    //public DataColumn DataColumn { get; }
    
   // public DataColumn? SecondDataColumn { get; set; }
    
    public DataViewColumnDataType DataType { get; }

    public string Title { get; }
    
    public string ShortName { get; set; }

    public string? NullValue { get; set; }

    public string? Format { get; set; }

    public DataViewColumnContentAlign? HorizontalAlign { get; set; }

    public Type? FilterComponentType { get; set; }

    public DataViewColumn(string dataColumnName, /*DataColumn dataColumn,*/ DataViewColumnDataType dataType, string title, string shortName/*, Type? filterComponentType*/)
    {
        DataColumnName = dataColumnName;
        //DataColumn = dataColumn;
        DataType = dataType;
        Title = title;
        ShortName = shortName;
        //FilterComponentType = filterComponentType;
    }
}
