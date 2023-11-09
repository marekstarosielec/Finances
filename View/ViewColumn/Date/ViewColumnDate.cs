using DataSource;

namespace View;

public class ViewColumnDate : ViewColumn
{
    public ViewColumnDate(string dataColumnName, string title, string shortName) : base(dataColumnName, ViewColumnDataType.Date, title, shortName)
    {
    }
}
