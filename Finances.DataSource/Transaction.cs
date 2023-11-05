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
            new DataColumn("DocumentId", DataType.Text)
            //new DataColumn("DocumentCategory", DataType.Text),
            //new DataColumn("DocumentInvoiceNumber", DataType.Text),
            //new DataColumn("DocumentNumber", DataType.Number)
            );

        var documentTable = new JsonDataSource("s:\\Lokalne\\Finanse\\Dane\\documents.json",
            new DataColumn("Id", DataType.Text),
            new DataColumn("Number", DataType.Number),
            new DataColumn("Pages", DataType.Number),
            new DataColumn("Description", DataType.Text),
            new DataColumn("Category", DataType.Text),
            new DataColumn("InvoiceNumber", DataType.Text),
            new DataColumn("Company", DataType.Text),
            new DataColumn("Person", DataType.Text),
            new DataColumn("Car", DataType.Text),
            new DataColumn("RelatedObject", DataType.Text),
            new DataColumn("Guarantee", DataType.Text),
            new DataColumn("CaseName", DataType.Text),
            new DataColumn("Settlement", DataType.Text),
            new DataColumn("TransactionId", DataType.Text),
            //new DataColumn("TransactionCategory", DataType.Text),
            //new DataColumn("TransactionAmount", DataType.Precision),
            //new DataColumn("TransactionCurrency", DataType.Text),
            //new DataColumn("TransactionBankInfo", DataType.Text),
            //new DataColumn("TransactionComment", DataType.Text),
            new DataColumn("Net", DataType.Precision),
            new DataColumn("Vat", DataType.Precision),
            new DataColumn("Gross", DataType.Precision),
            new DataColumn("Currency", DataType.Text)
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

        var sortd = new Dictionary<DataColumn, bool>
        {
            { documentTable.Columns["Number"], true }
        };
        var dataQueryd = new DataQuery
        {
            Sort = sort,
            PageSize = 100
        };
        var td = await documentTable.GetDataView(dataQueryd);

        var transactionWithDocument = new JoinedDataSource(transactionTable, documentTable, "DocumentId",
            new DataColumnMapping("Id", null),
            new DataColumnMapping("Number", "DocumentNumber"),
            new DataColumnMapping("Pages", null),
            new DataColumnMapping("Description", null),
            new DataColumnMapping("Category", null),
            new DataColumnMapping("InvoiceNumber", null),
            new DataColumnMapping("Company", null),
            new DataColumnMapping("Person", null),
            new DataColumnMapping("Car", null),
            new DataColumnMapping("RelatedObject", null),
            new DataColumnMapping("Guarantee", null),
            new DataColumnMapping("CaseName", null),
            new DataColumnMapping("Settlement", null),
            new DataColumnMapping("TransactionId", null),
            new DataColumnMapping("Net", null),
            new DataColumnMapping("Vat", null),
            new DataColumnMapping("Gross", null),
            new DataColumnMapping("Currency", null)
            );

        var sort2 = new Dictionary<DataColumn, bool>
        {
            { transactionWithDocument.Columns["DocumentNumber"], true }
        };
        var dataQuery2 = new DataQuery
        {
            Sort = sort2,
            Filter= new Dictionary<DataColumn, DataColumnFilter>
            {
                { transactionWithDocument.Columns["Date"], new DataColumnFilter{ DateFrom = new DateTime(2023,10,1), DateTo = new DateTime(2023,10,1)} }
            },
            PageSize = 100
        };
        var t3 = transactionWithDocument.GetDataView(dataQuery2);

    }
}