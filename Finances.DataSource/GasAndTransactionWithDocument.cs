using DataSource;

namespace Finances.DataSource;

public partial class DataSourceFactory
{
    private IDataSource? _gasAndtransactionWithDocument = null;
    public IDataSource GasAndTransactionWithDocument
    {
        get
        {
            _gasAndtransactionWithDocument ??= new UnionedDataSource(Gas, TransactionWithDocument, 
                    new Dictionary<DataColumn, DataColumnFilter>
                    {
                        { TransactionWithDocument.Columns["Category"], new DataColumnFilter { StringValue = new List<string> { "Gaz" } } }
                    },
                    new DataColumnUnionMapping("Id", "Id", "Id"),
                    new DataColumnUnionMapping("Date", "Date", "Date"),
                    new DataColumnUnionMapping("Meter", "Meter", null),
                    new DataColumnUnionMapping("Comment", "Comment", "Comment"),
                    new DataColumnUnionMapping("Amount", null, "Amount"),
                    new DataColumnUnionMapping("Currency", null, "Currency")
                    );
            return _gasAndtransactionWithDocument;
        }
    }
}
