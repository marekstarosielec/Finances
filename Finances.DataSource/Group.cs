using DataSource;

namespace Finances.DataSource;

public partial class DataSourceFactory
{
    private IDataSource? _group = null;
    public IDataSource Group
    {
        get
        {
            _group ??= new JsonDataSource(Path.Combine(_dataFilePath!, "group.json"),
                new IdDataColumn(),
                new DataColumn("GroupId", ColumnDataType.Text),
                new DataColumn("DataViewName", ColumnDataType.Text),
                new DataColumn("RowId", ColumnDataType.Text)
                );
            return _group;
        }
    }
}