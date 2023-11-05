using System.Text.Json.Nodes;

namespace DataSource.Json;

internal class NodesList
{
    internal IEnumerable<JsonNode> Nodes { get; set; }

    internal int Count { get; set; }

    internal NodesList(IEnumerable<JsonNode> nodes)
    {
        Nodes = nodes;
    }
}
