using DataSource.Json;
using DataSource;

namespace Finances.DataSource;

public partial class DataSourceFactory
{
    public IDataSource Gas = new JsonDataSource("s:\\Lokalne\\Finanse\\Dane\\gas.json",
        new DataColumn("Id", DataType.Text),
        new DataColumn("Date", DataType.Date),
        new DataColumn("Meter", DataType.Precision),
        new DataColumn("Comment", DataType.Text)
        );
}