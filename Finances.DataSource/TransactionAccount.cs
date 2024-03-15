using DataSource;

namespace Finances.DataSource;

public partial class DataSourceFactory
{
    private IDataSource? _transactionAccount = null;
    public IDataSource TransactionAccount
    {
        get
        {
            _transactionAccount ??= new JsonDataSource(_dataFilePath!, "transaction-accounts.json",
                includeGroups: true,
                new IdDataColumn(),
                new DataColumn("Title", ColumnDataType.Text),
                new DataColumn("Currency", ColumnDataType.Text),
                new DataColumn("Deleted", ColumnDataType.Bool)
                );
            return _transactionAccount;
        }
    }
}