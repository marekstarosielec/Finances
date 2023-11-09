using DataSource;

namespace View;

public class ViewColumnText : ViewColumn
{
    public ViewColumnText(string dataColumnName, string title, string shortName) : base(dataColumnName, ViewColumnDataType.Text, title, shortName)
    {
    }
}
