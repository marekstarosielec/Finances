namespace DataSource.Tests.DataSourceCache;

public class DataSourceRelationsFinderTests
{
    [Fact]
    public void FindAllRelatedElements_WithDirectRelations_ReturnsCorrectRelations()
    {
        // Arrange
        var relations = new Dictionary<string, List<string>>
        {
            { "A", new List<string> { "B" } },
            { "B", new List<string> { "C" } }
        };
        var finder = new DataSourceRelationsFinder(relations);

        // Act
        var relatedElements = finder.FindAllRelatedElements("A");

        // Assert
        Assert.Contains("B", relatedElements);
        Assert.Contains("C", relatedElements);
        Assert.Equal(2, relatedElements.Count);
    }

    [Fact]
    public void FindAllRelatedElements_WithNoRelations_ReturnsEmpty()
    {
        // Arrange
        var relations = new Dictionary<string, List<string>>
        {
            { "A", new List<string>() }
        };
        var finder = new DataSourceRelationsFinder(relations);

        // Act
        var relatedElements = finder.FindAllRelatedElements("A");

        // Assert
        Assert.Empty(relatedElements);
    }

    [Fact]
    public void FindAllRelatedElements_WithCircularRelations_DoesNotCauseInfiniteLoop()
    {
        // Arrange
        var relations = new Dictionary<string, List<string>>
        {
            { "A", new List<string> { "B" } },
            { "B", new List<string> { "A" } } // Circular relation
        };
        var finder = new DataSourceRelationsFinder(relations);

        // Act
        var relatedElements = finder.FindAllRelatedElements("A");

        // Assert
        Assert.Contains("B", relatedElements);
        Assert.Single(relatedElements); // Ensure no duplicates or infinite loops
    }
}
