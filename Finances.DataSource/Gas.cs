﻿using DataSource.Json;
using DataSource;

namespace Finances.DataSource;

public partial class DataSourceFactory
{
    private IDataSource? _gas = null;
    public IDataSource Gas
    {
        get
        {
            _gas ??= new JsonDataSource(Path.Combine(_dataFilePath!, "gas.json"),
                new DataColumn("Id", ColumnDataType.Text),
                new DataColumn("Date", ColumnDataType.Date),
                new DataColumn("Meter", ColumnDataType.Precision),
                new DataColumn("Comment", ColumnDataType.Text)
                );
            return _gas;
        }
    }
}