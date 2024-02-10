using DataSource.Json;
using DataSource;

namespace Finances.DataSource;

public partial class DataSourceFactory
{
    private IDataSource? _document = null;
    public IDataSource Document
    {
        get
        {
            _document ??= new JsonDataSource(Path.Combine(_dataFilePath!, "documents.json"),
                new DataColumn("Id", ColumnDataType.Text),
                new DataColumn("Date", ColumnDataType.Date),
                new DataColumn("Number", ColumnDataType.Number),
                new DataColumn("Pages", ColumnDataType.Number),
                new DataColumn("Description", ColumnDataType.Text),
                new DataColumn("Category", ColumnDataType.Text),
                new DataColumn("InvoiceNumber", ColumnDataType.Text),
                new DataColumn("Company", ColumnDataType.Text),
                new DataColumn("Person", ColumnDataType.Text),
                new DataColumn("Car", ColumnDataType.Text),
                new DataColumn("RelatedObject", ColumnDataType.Text),
                new DataColumn("Guarantee", ColumnDataType.Text),
                new DataColumn("CaseName", ColumnDataType.Text),
                new DataColumn("Settlement", ColumnDataType.Text),
                new DataColumn("TransactionId", ColumnDataType.Text),
                //new DataColumn("TransactionCategory", ColumnDataType.Text),
                //new DataColumn("TransactionAmount", ColumnDataType.Precision),
                //new DataColumn("TransactionCurrency", ColumnDataType.Text),
                //new DataColumn("TransactionBankInfo", ColumnDataType.Text),
                //new DataColumn("TransactionComment", ColumnDataType.Text),
                new DataColumn("Net", ColumnDataType.Precision),
                new DataColumn("Vat", ColumnDataType.Precision),
                new DataColumn("Gross", ColumnDataType.Precision),
                new DataColumn("Currency", ColumnDataType.Text),
                //new DataColumn("GroupId", ColumnDataType.Text),
                new DataColumn("FileLink", ColumnDataType.Text, CreateDocumentFullName)
                );
            return _document;
        }
    }

    private string? CreateDocumentFullName(DataRow row)
    {
        var number = row["Number"].OriginalValue?.ToString();
        if (string.IsNullOrWhiteSpace(number))
            return null;

        var fileNumber = $"MX{number.PadLeft(5, '0')}.zip";
        return Path.Combine(_dataFilePath!, "documents", fileNumber);
    }
}


