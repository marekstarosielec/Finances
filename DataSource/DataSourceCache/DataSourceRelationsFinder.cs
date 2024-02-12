namespace DataSource;

internal class DataSourceRelationsFinder
{
    private readonly Dictionary<string, List<string>> _relations;

    public DataSourceRelationsFinder(Dictionary<string, List<string>> relations)
    {
        _relations = relations;
    }

    public HashSet<string> FindAllRelatedElements(string element)
    {
        var relatedElements = new HashSet<string>();
        Traverse(element, element, relatedElements);
        return relatedElements;
    }

    private void Traverse(string rootElement, string element, HashSet<string> relatedElements)
    {
        if (!_relations.ContainsKey(element) || _relations[element].Count == 0)
            return;
        
        foreach (var relatedElement in _relations[element])
            if (relatedElement != rootElement && relatedElements.Add(relatedElement))
                Traverse(rootElement, relatedElement, relatedElements);
    }
}