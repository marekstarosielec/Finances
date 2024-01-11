namespace DataSource.Document;

public interface IDocumentManager
{
    bool IsDocumentDecompressed(string fileName);

    void DecompressDocument(string fileName, string password);
}

public class DocumentManager : IDocumentManager
{
    public bool IsDocumentDecompressed(string fileName)
    {
        var path = Path.GetDirectoryName(fileName);
        var subFolder = Path.GetFileNameWithoutExtension(fileName);
        if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(subFolder))
            throw new InvalidOperationException("Failed to generate proper path");
        var uncompressedPath = Path.Combine(path, subFolder);
        return Directory.Exists(uncompressedPath);
    }

    public void DecompressDocument(string fileName, string password)
    {
        //throw new NotImplementedException();
    }
}
