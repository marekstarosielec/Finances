using DataSource;

namespace View;

public abstract class ViewColumn
{
    public string DataColumnName { get; }
    //public DataColumn DataColumn { get; }
    
   // public DataColumn? SecondDataColumn { get; set; }
    
    public ViewColumnDataType DataType { get; }

    public string Title { get; }
    
    public string ShortName { get; set; }

    public string? NullValue { get; set; }

    public string? Format { get; set; }

    public ViewColumnContentAlign? HorizontalAlign { get; set; }

    public Type? FilterComponentType { get; set; }

    public ViewColumn(string dataColumnName, /*DataColumn dataColumn,*/ ViewColumnDataType dataType, string title, string shortName/*, Type? filterComponentType*/)
    {
        DataColumnName = dataColumnName;
        //DataColumn = dataColumn;
        DataType = dataType;
        Title = title;
        ShortName = shortName;
        //FilterComponentType = filterComponentType;
    }
}
