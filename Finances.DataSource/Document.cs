using DataSource.Json;
using DataSource;

namespace Finances.DataSource;

public partial class DataSourceFactory
{
    public IDataSource Document = new JsonDataSource("s:\\Lokalne\\Finanse\\Dane\\documents.json",
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
}


