﻿using DataSource;

namespace Finances.DataSource;

public partial class DataSourceFactory
{
    private IDataSource? _gasUnionTransaction = null;
    public IDataSource GasUnionTransaction
    {
        get
        {
            _gasUnionTransaction ??= new UnionedDataSource(Gas, Transaction,
                    new Dictionary<DataColumn, DataColumnFilter>
                    {
                        { Transaction.Columns["Category"], new DataColumnFilter { StringValue = new List<string> { "Gaz" } } }
                    },
                    new DataColumnUnionMapping("Id", "Id", "Id"),
                    new DataColumnUnionMapping("Date", "Date", "Date"),
                    new DataColumnUnionMapping("Meter", "Meter", null),
                    new DataColumnUnionMapping("Comment", "Comment", "Comment"),
                    new DataColumnUnionMapping("Amount", null, "Amount"),
                    new DataColumnUnionMapping("Currency", null, "Currency")//,
                    //new DataColumnUnionMapping("FileLink", null, "FileLink")
                    );
            return _gasUnionTransaction;
        }
    }
}
