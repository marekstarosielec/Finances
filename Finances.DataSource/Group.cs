using DataSource;
using DataSource.Json;

namespace Finances.DataSource;

public partial class DataSourceFactory
{
    private IDataSource? _group = null;
    public IDataSource Group
    {
        get
        {
            _group ??= new JsonDataSource(Path.Combine(_dataFilePath!, "group.json"),
                new DataColumn("Id", ColumnDataType.Text),
                new DataColumn("GroupId", ColumnDataType.Text),
                new DataColumn("DataViewName", ColumnDataType.Date),
                new DataColumn("RowId", ColumnDataType.Text)
                );
            return _group;
        }
    }
}