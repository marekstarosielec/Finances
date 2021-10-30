using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;

namespace FinancesApi.Services
{
	public interface ICompressionService
    {
		void Compress(List<string> files, string archiveName);
        void Decompress(string archiveName);
    }

    public class CompressionService: ICompressionService
	{
        public void Compress(List<string> files, string archiveName)
        {
            using (var file = ZipFile.Create(archiveName))
            {
                file.BeginUpdate();
                files.ForEach(x => file.Add(x, Path.GetFileName(x)));
                file.Password = "abc";
                file.UseZip64 = UseZip64.On;
                file.CommitUpdate();
                file.Close();
            }
        }

        public void Decompress(string archiveName)
        {
            ZipFile file = null;
            try
            {
                FileStream fs = File.OpenRead(archiveName);
                file = new ZipFile(fs);
                file.Password = "abc";
                var outputFolder = Path.GetDirectoryName(archiveName);

                foreach (ZipEntry zipEntry in file)
                {
                    byte[] buffer = new byte[4096];
                    Stream zipStream = file.GetInputStream(zipEntry);

                    using (FileStream streamWriter = File.Create(Path.Combine(outputFolder, zipEntry.Name)))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }
            }
            finally
            {
                if (file != null)
                {
                    file.IsStreamOwner = true; // Makes close also shut the underlying stream
                    file.Close(); // Ensure we release resources
                }
            }
        }
    }
}
