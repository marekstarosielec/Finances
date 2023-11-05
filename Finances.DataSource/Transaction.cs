using DataSource;
using DataSource.Json;

namespace Finances.DataSource;

public class Transaction
{
    public async Task Test()
    {
        var transactionTable = new JsonDataSource("s:\\Lokalne\\Finanse\\Dane\\transactions.json", 
            new DataColumn("Id", DataType.Text),
            new DataColumn("ScrappingDate", DataType.Date),
            new DataColumn("Status", DataType.Text),
            new DataColumn("Source", DataType.Text),
            new DataColumn("Date", DataType.Date),
            new DataColumn("Account", DataType.Text),
            new DataColumn("Category", DataType.Text),
            new DataColumn("Amount", DataType.Precision),
            new DataColumn("Title", DataType.Text),
            new DataColumn("Description", DataType.Text),
            new DataColumn("Text", DataType.Text),
            new DataColumn("BankInfo", DataType.Text),
            new DataColumn("Comment", DataType.Text),
            new DataColumn("Currency", DataType.Text),
            new DataColumn("Details", DataType.Text),
            new DataColumn("Person", DataType.Text),
            new DataColumn("CaseName", DataType.Text),
            new DataColumn("Settlement", DataType.Text),
            new DataColumn("DocumentId", DataType.Text),
            new DataColumn("DocumentCategory", DataType.Text),
            new DataColumn("DocumentInvoiceNumber", DataType.Text),
            new DataColumn("DocumentNumber", DataType.Number)
            );
        var sort = new Dictionary<DataColumn, bool>
        {
            { transactionTable.Columns["Date"], true }
        };
        var dataQuery = new DataQuery
        {
            Sort = sort,
            PageSize = 100
        };
        var t = await transactionTable.GetDataView(dataQuery);
    }
}