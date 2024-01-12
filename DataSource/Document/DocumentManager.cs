namespace DataSource.Document;

public interface IDocumentManager
{
    bool IsDocumentDecompressed(string fileName);

    void DecompressDocument(string fileName, string password);
    
    IEnumerable<string> GetDecompressedInfo(string fileName);
}

public class DocumentManager : IDocumentManager
{
    private readonly ICompressionService _compressionService;
    private readonly DocumentManagerOptions _options;

    public DocumentManager(ICompressionService compressionService, DocumentManagerOptions options)
    {
        _compressionService = compressionService;
        _options = options;

        if (string.IsNullOrWhiteSpace(options?.DecompressionPath))
            throw new ArgumentNullException(nameof(options));
    }

    public bool IsDocumentDecompressed(string fileName) => Directory.Exists(GetDecompressedPath(fileName));

    public IEnumerable<string> GetDecompressedInfo(string fileName)
    {
        if (!IsDocumentDecompressed(fileName))
            throw new InvalidOperationException("File is not decompressed");

        foreach (var file in Directory.GetFiles(GetDecompressedPath(fileName)))
            yield return Path.Combine(Path.GetFileNameWithoutExtension(fileName), Path.GetFileName(file));
    }

    public void DecompressDocument(string fileName, string password)
    {
        _compressionService.Decompress(fileName, password, _options.DecompressionPath!);
    }

    private string GetDecompressedPath(string fileName)
    {
        var path = Path.GetDirectoryName(fileName);
        var subFolder = Path.GetFileNameWithoutExtension(fileName);
        if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(subFolder))
            throw new InvalidOperationException("Failed to generate proper path");
        return Path.Combine(_options.DecompressionPath!, subFolder);
    }
}
