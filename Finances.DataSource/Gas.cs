using DataSource;

namespace Finances.DataSource;

public partial class DataSourceFactory
{
    private IDataSource? _gas = null;
    public IDataSource Gas
    {
        get
        {
            _gas ??= new JsonDataSource(_dataFilePath!, "gas.json",
                new IdDataColumn(),
                new DataColumn("Date", ColumnDataType.Date),
                new DataColumn("Meter", ColumnDataType.Precision),
                new DataColumn("Comment", ColumnDataType.Text)
                );
            return _gas;
        }
    }
}