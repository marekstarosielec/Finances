using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FinancesApi.Services
{
    public interface IFileService
    {
        void MakeNonCompressedBackups(Action<string> beforeFileCopy = null);
        void MakeCompressedBackups(string password, Action<string> beforeFileCompress = null, Action<string> beforeFileCopy = null);
        string DecompressFile(int number, string password);
    }

    public class FileService : IFileService
    {
        private string _fileFolder;
        private readonly IConfiguration _configuration;

        private readonly ICompressionService _compressionService;
        
        private readonly List<string> _fileBackupLocations;
        private readonly List<string> _datasetBackupLocations;

        public FileService(
            IConfiguration configuration, 
            ICompressionService compressionService)
        {
            _configuration = configuration;
            
            _fileFolder = Path.Combine(configuration.GetValue<string>("DatasetPath"), "Dokumenty");
            
            _fileBackupLocations = configuration.GetSection("DatasetFilesBackups").Get<List<string>>();
            _datasetBackupLocations = configuration.GetSection("DatasetBackups").Get<List<string>>(); 
            
            _compressionService = compressionService;
        }

        public void MakeNonCompressedBackups(Action<string> beforeFileCopy = null)
        {
            foreach (var location in _fileBackupLocations)
            {
                var destinationFolderMain = Path.Combine(location, "Dokumenty");
                var existingFolders = Directory.EnumerateDirectories(destinationFolderMain).ToDictionary<string, string>(_ => Path.GetFileName(_));
                foreach (var folder in Directory.EnumerateDirectories(_fileFolder))
                {
                    var folderName = Path.GetFileName(folder);
                    var destinationLocation = Path.Combine(destinationFolderMain, folderName);
                    
                    if (existingFolders.ContainsKey(folderName))
                        continue; //File is already there
                    
                    Directory.CreateDirectory(destinationLocation);
                    IOUtilities.Copy(folder, destinationLocation, beforeFileCopy, false);

                };
            }
        }

        public void MakeCompressedBackups(string password, Action<string> beforeFileCompress = null, Action<string> beforeFileCopy = null)
        {
            foreach (var element in Directory.EnumerateDirectories(_fileFolder))
            {
                var compressedFileName = Path.Combine(_fileFolder, $"{Path.GetFileName(element)}.zip");
                if (File.Exists(compressedFileName))
                {
                    Directory.Delete(element, true);
                    continue;
                }
                beforeFileCompress?.Invoke(compressedFileName);
                _compressionService.Compress(compressedFileName, password, element);
                Directory.Delete(element, true);
            }
            foreach (var location in _datasetBackupLocations)
            {
                var destinationLocation = Path.Combine(location, "Dokumenty");
                IOUtilities.Copy(_fileFolder, destinationLocation, beforeFileCopy, false);
            };
        }

        public string DecompressFile(int number, string password)
        {
            var numberName = $"MX{number.ToString().PadLeft(5, '0')}";
            var destinationPath = Path.Combine(_fileFolder, numberName);
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
                var sourceFile = Path.Combine(_fileFolder, $"{numberName}.zip");
                var destinationFile = Path.Combine(_fileFolder, numberName, $"{numberName}.zip");
                File.Copy(sourceFile, destinationFile);
                _compressionService.Decompress(destinationFile, password);
                File.Delete(destinationFile);
            }
            var files = Directory.GetFiles(destinationPath);
            var result = files.Length == 1 ? files.First() : destinationPath;
            if (result.StartsWith(_fileFolder))
                result = result.Substring(_fileFolder.Length);
            return result;
        }
    }
}
