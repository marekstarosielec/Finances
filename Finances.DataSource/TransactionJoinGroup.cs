using DataSource;

namespace Finances.DataSource;

public partial class DataSourceFactory
{
    private IDataSource? _transactionJoinGroup = null;
    public IDataSource TransactionJoinGroup
    {
        get
        {
            _transactionJoinGroup ??= new JoinedDataSource(Transaction, Group, "RowId",
                new DataColumnJoinMapping("Id", null)
            );
            return _transactionJoinGroup;
        }
    }
}