﻿using DataSource;
using DataSource.Json;

namespace Finances.DataSource;

public partial class DataSourceFactory
{
    private IDataSource? _transaction = null;
    public IDataSource Transaction
    {
        get
        {
            _transaction ??= new JsonDataSource(Path.Combine(_dataFilePath!, "transactions.json"),
                new DataColumn("Id", ColumnDataType.Text),
                new DataColumn("ScrappingDate", ColumnDataType.Date),
                new DataColumn("Status", ColumnDataType.Text),
                new DataColumn("Source", ColumnDataType.Text),
                new DataColumn("Date", ColumnDataType.Date),
                new DataColumn("Account", ColumnDataType.Text),
                new DataColumn("Category", ColumnDataType.Text),
                new DataColumn("Amount", ColumnDataType.Precision),
                new DataColumn("Title", ColumnDataType.Text),
                new DataColumn("Description", ColumnDataType.Text),
                new DataColumn("Text", ColumnDataType.Text),
                new DataColumn("BankInfo", ColumnDataType.Text),
                new DataColumn("Comment", ColumnDataType.Text),
                new DataColumn("Currency", ColumnDataType.Text),
                new DataColumn("Details", ColumnDataType.Text),
                new DataColumn("Person", ColumnDataType.Text),
                new DataColumn("CaseName", ColumnDataType.Text),
                new DataColumn("Settlement", ColumnDataType.Text),
                new DataColumn("DocumentId", ColumnDataType.Text),
                new DataColumn("GroupId", ColumnDataType.Text)
                //new DataColumn("DocumentCategory", ColumnDataType.Text),
                //new DataColumn("DocumentInvoiceNumber", ColumnDataType.Text),
                //new DataColumn("DocumentNumber", ColumnDataType.Number)
                );
            return _transaction;
        }
    }
}