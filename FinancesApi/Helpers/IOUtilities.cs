using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

public static class IOUtilities
{
    public static void Copy(string fromFolder, string toFolder, Action<string> beforeFileCopy = null, bool overwrite = false)
    {
        var _createdFolders = new ConcurrentDictionary<string, string>();
        Directory
            .EnumerateFiles(fromFolder, "*.*", SearchOption.AllDirectories)
            .AsParallel()
            .ForAll(from =>
            {
                var to = from.Replace(fromFolder, toFolder);

                // Create directories if required
                var toSubFolder = Path.GetDirectoryName(to);
                if (!string.IsNullOrWhiteSpace(toSubFolder) && !_createdFolders.ContainsKey(toSubFolder))
                {
                    Directory.CreateDirectory(toSubFolder);
                    _createdFolders[toSubFolder] = toSubFolder;
                }

                if (overwrite || !File.Exists(to))
                {
                    beforeFileCopy?.Invoke(to);
                    File.Copy(from, to, overwrite);
                }
            });
    }
}
