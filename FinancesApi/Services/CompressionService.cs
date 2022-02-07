using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using System.Collections.Generic;
using System.IO;

namespace FinancesApi.Services
{
	public interface ICompressionService
    {
		void Compress(List<string> files, string archiveName, string password);
        void Compress(string archiveName, string password, string sourceDirectory);
        void Decompress(string archiveName, string password);
        void Decompress(string archiveName, string password, string outputFolder);
    }

    public class CompressionService: ICompressionService
	{
        public void Compress(List<string> files, string archiveName, string password)
        {
            if (File.Exists(archiveName))
                File.Delete(archiveName);
            var rootFolder = Path.GetDirectoryName(archiveName);
            using var zip = ZipFile.Create(archiveName);
            zip.BeginUpdate();
            files.ForEach(x => zip.Add(x, x.Substring(rootFolder.Length + 1)));
            if (!string.IsNullOrWhiteSpace(password))
                zip.Password = password;
            zip.UseZip64 = UseZip64.On;
            zip.CommitUpdate();
            zip.Close();
        }

        public void Compress(string archiveName, string password, string sourceDirectory)
        {
            if (File.Exists(archiveName))
                File.Delete(archiveName);

            var zip = new FastZip();
            zip.CompressionLevel = Deflater.CompressionLevel.BEST_SPEED;
            zip.CreateEmptyDirectories = true;
            zip.EntryEncryptionMethod = ZipEncryptionMethod.AES256;
            zip.Password = password;
            zip.RestoreAttributesOnExtract = true;
            zip.RestoreDateTimeOnExtract = true;
            zip.UseZip64 = UseZip64.On;
            zip.CreateZip(archiveName, sourceDirectory, true, null);
        }

        public void Decompress(string archiveName, string password)
        {
            var outputFolder = Path.GetDirectoryName(archiveName);
            Decompress(archiveName, password, outputFolder);
        }

        public void Decompress(string archiveName, string password, string outputFolder)
        {
            ZipFile file = null;
            try
            {
                FileStream fs = File.OpenRead(archiveName);
                file = new ZipFile(fs);
                file.Password = password;
                Directory.CreateDirectory(outputFolder);

                foreach (ZipEntry zipEntry in file)
                {
                    if (!zipEntry.IsFile)
                        continue;

                    byte[] buffer = new byte[4096];
                    Stream zipStream = file.GetInputStream(zipEntry);

                    var subdirectory = Path.GetDirectoryName(zipEntry.Name);
                    if (!string.IsNullOrWhiteSpace(subdirectory))
                        Directory.CreateDirectory(Path.Combine(outputFolder, subdirectory));
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
}
