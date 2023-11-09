using Microsoft.Extensions.Configuration;

namespace Finances.DataSource;

public partial class DataSourceFactory
{
    private string? _dataFilePath;

    public DataSourceFactory(IConfiguration configuration)
    {
        _dataFilePath = configuration.GetValue<string>("DatasetPath");
        if (string.IsNullOrWhiteSpace(_dataFilePath))
            throw new InvalidOperationException("DatasetPath configuration setting is invalid");
    }
}