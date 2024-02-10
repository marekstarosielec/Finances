using DataSource;

namespace Finances.DataSource;

public partial class DataSourceFactory
{
    private IDataSource? _documentJoinGroup = null;
    public IDataSource DocumentJoinGroup
    {
        get
        {
            _documentJoinGroup ??= new JoinedDataSource(Document, Group, "RowId",
                new DataColumnJoinMapping("Id", null)
            );
            return _documentJoinGroup;
        }
    }
}