using DataSource;

namespace Finances.DataSource;

public partial class DataSourceFactory
{
    private IDataSource? _gasAndtransactionWithDocument = null;
    public IDataSource GasAndTransactionWithDocument
    {
        get
        {
            _gasAndtransactionWithDocument ??= new JoinedDataSource(Transaction, Document, "DocumentId",
                    new DataColumnJoinMapping("Id", null),
                    new DataColumnJoinMapping("Number", "DocumentNumber"),
                    new DataColumnJoinMapping("Pages", null),
                    new DataColumnJoinMapping("Description", null),
                    new DataColumnJoinMapping("Category", null),
                    new DataColumnJoinMapping("InvoiceNumber", null),
                    new DataColumnJoinMapping("Company", null),
                    new DataColumnJoinMapping("Person", null),
                    new DataColumnJoinMapping("Car", null),
                    new DataColumnJoinMapping("RelatedObject", null),
                    new DataColumnJoinMapping("Guarantee", null),
                    new DataColumnJoinMapping("CaseName", null),
                    new DataColumnJoinMapping("Settlement", null),
                    new DataColumnJoinMapping("TransactionId", null),
                    new DataColumnJoinMapping("Net", null),
                    new DataColumnJoinMapping("Vat", null),
                    new DataColumnJoinMapping("Gross", null),
                    new DataColumnJoinMapping("Currency", null)
                    );
            return _gasAndtransactionWithDocument;
        }
    }
}
