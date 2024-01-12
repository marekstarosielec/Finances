using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace DataSource;

public interface ICompressionService
{
    void Compress(List<string> files, string archiveName, string password);
    void Decompress(string archiveName, string password, string outputFolder);
}

public class CompressionService : ICompressionService
{
    public void Compress(List<string> files, string archiveName, string password)
    {
        using var file = ZipFile.Create(archiveName);
        file.BeginUpdate();
        files.ForEach(x => file.Add(x, Path.GetFileName(x)));
        file.Password = password;
        file.UseZip64 = UseZip64.On;
        file.CommitUpdate();
        file.Close();
    }

    public void Decompress(string archiveName, string password, string outputFolder)
    {
        ZipFile? file = null;
        try
        {
            FileStream fs = File.OpenRead(archiveName);
            file = new ZipFile(fs)
            {
                Password = password
            };
            if (string.IsNullOrWhiteSpace(outputFolder))
                throw new InvalidOperationException("Failed to get decompression path");
            outputFolder = Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(archiveName));
            Directory.CreateDirectory(outputFolder);
            foreach (ZipEntry zipEntry in file)
            {
                byte[] buffer = new byte[4096];
                Stream zipStream = file.GetInputStream(zipEntry);

                using FileStream streamWriter = File.Create(Path.Combine(outputFolder, zipEntry.Name));
                StreamUtils.Copy(zipStream, streamWriter, buffer);
            }
        }
        finally
        {
            if (file != null)
            {
                file.IsStreamOwner = true;
                file.Close();
            }
        }
    }
}