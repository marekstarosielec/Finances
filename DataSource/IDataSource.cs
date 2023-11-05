using System.Text.Json.Nodes;

namespace DataSource;

public interface IDataSource
{
    Task<IEnumerable<JsonNode>> GetDataView(DataView view);
}