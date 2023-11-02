namespace FinancesBlazor.DataAccess;

public record JoinDefinition
{
    public string FileName { get; }

    public string JoinColumn { get; }
    public string JoinParentNodeName { get; }

    public JoinDefinition(string fileName, string joinColumn, string joinParentNodeName)
    {
        FileName = fileName;
        JoinColumn = joinColumn;
        JoinParentNodeName = joinParentNodeName;
    }
}
