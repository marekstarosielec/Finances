using FinancesApi.DataFiles;
using FinancesApi.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace FinancesApi.Services
{
    public interface IDocumentDatasetService
    {
        DocumentDatasetInfo GetInfo();

        DocumentDatasetInfo Open(string password);

        DocumentDatasetInfo Close(string password, bool makeBackups);
    }

    public class DocumentDatasetService : IDocumentDatasetService
    {
        private string _basePath;
        private string _archiveFile;
        private string _decompressedFolder;
        private readonly IConfiguration _configuration;

        private readonly ICompressionService _compressionService;
        private readonly DocumentDatasetInfoDataFile _documentDatasetInfoDataFile;

        private readonly List<string> _fileBackupLocations;
        private readonly List<string> _datasetBackupLocations;

        public DocumentDatasetService(
            IConfiguration configuration, 
            ICompressionService compressionService,
            DocumentDatasetInfoDataFile documentDatasetInfoDataFile)
        {
            _configuration = configuration;
            
            _basePath = Path.Combine(configuration.GetValue<string>("DatasetPath"));
            _archiveFile = Path.Combine(_basePath, "Dokumenty.zip");
            _decompressedFolder = Path.Combine(_basePath, "Dokumenty");

            _fileBackupLocations = configuration.GetSection("DatasetFilesBackups").Get<List<string>>();
            _datasetBackupLocations = configuration.GetSection("DatasetBackups").Get<List<string>>(); 
            
            _compressionService = compressionService;
            _documentDatasetInfoDataFile = documentDatasetInfoDataFile;
            _documentDatasetInfoDataFile.Load();
        }

        public DocumentDatasetInfo GetInfo()
        {
            try
            {
                var local = new DocumentDatasetInfoDataFile(_configuration);
                local.Load();
                return local.Value;
            }
            catch (Exception e)
            {
                return new DocumentDatasetInfo { State = DatasetState.UnknownError, Message = e.Message };
            }
        }

        public DocumentDatasetInfo Open(string password)
        {
            _documentDatasetInfoDataFile.Load();
            if (_documentDatasetInfoDataFile.Value.State != DatasetState.Closed && _documentDatasetInfoDataFile.Value.State != DatasetState.OpeningError)
                return _documentDatasetInfoDataFile.Value;
            _documentDatasetInfoDataFile.Value.State = DatasetState.Opening;
            _documentDatasetInfoDataFile.Value.Message = string.Empty;
            _documentDatasetInfoDataFile.Save();

            new Thread(() => {
                DecompressBackup(password);
            }).Start();
            return _documentDatasetInfoDataFile.Value;
        }
            

        public DocumentDatasetInfo Close(string password, bool makeBackups)
        {
            _documentDatasetInfoDataFile.Load();
            if (_documentDatasetInfoDataFile.Value.State != DatasetState.Opened && _documentDatasetInfoDataFile.Value.State != DatasetState.ClosingError)
                return _documentDatasetInfoDataFile.Value;
            _documentDatasetInfoDataFile.Value.State = DatasetState.Closing;
            _documentDatasetInfoDataFile.Value.Message = string.Empty;
            _documentDatasetInfoDataFile.Save();

            new Thread(() =>
            {
                try
                {
                    if (makeBackups) MakeBackups(password);
                    RemoveSource();

                    _documentDatasetInfoDataFile.Load();
                    _documentDatasetInfoDataFile.Value.State = DatasetState.Closed;
                    if (makeBackups)
                        _documentDatasetInfoDataFile.Value.LastCloseDate = DateTime.Now;
                    _documentDatasetInfoDataFile.Value.Message = string.Empty;
                    _documentDatasetInfoDataFile.Save();
                }
                catch (Exception e)
                {
                    _documentDatasetInfoDataFile.Load();
                    _documentDatasetInfoDataFile.Value.State = DatasetState.ClosingError;
                    _documentDatasetInfoDataFile.Value.Message = e.Message;
                    _documentDatasetInfoDataFile.Save();
                }
            }).Start();
            
            return _documentDatasetInfoDataFile.Value;
        }

        private void MakeBackups(string password)
        {
            MakeNonCompressedBackups();
            MakeCompressedBackups(password);
        }

        private void MakeNonCompressedBackups()
        {
            foreach (var location in _fileBackupLocations)
            {
                var destinationLocation = Path.Combine(location, "Dokumenty");
                Directory.CreateDirectory(destinationLocation);
                foreach (var dataFile in Directory.GetFiles(destinationLocation))
                {
                    var destinationFile = Path.Combine(destinationLocation, Path.GetFileName(dataFile));
                    try
                    {
                        if (File.Exists(destinationFile) && new FileInfo(destinationFile).Length == new FileInfo(dataFile).Length)
                            continue;

                        _documentDatasetInfoDataFile.Load();
                        _documentDatasetInfoDataFile.Value.Message = $"Kopiowanie {Path.GetFileName(dataFile)} do {destinationLocation}";
                        _documentDatasetInfoDataFile.Save(); 
                        File.Copy(dataFile, destinationFile, true);
                    }
                    catch (Exception e)
                    {
                        //Failure during copy is critical.
                        throw new Exception($"Nie udało się skopiować zbioru do {destinationFile}", e);
                    }
                };
                File.Copy(_documentDatasetInfoDataFile.DataFile.FileNameWithLocation, Path.Combine(location, _documentDatasetInfoDataFile.DataFile.FileName), true);
            };
        }

        private void MakeCompressedBackups(string password)
        {
            _documentDatasetInfoDataFile.Load();
            _documentDatasetInfoDataFile.Value.Message = "Tworzenie archiwum";
            _documentDatasetInfoDataFile.Save();
            _compressionService.Compress(_archiveFile, password, _decompressedFolder);

            _datasetBackupLocations.ForEach(destinationLocation =>
            {
                try
                {
                    _documentDatasetInfoDataFile.Load();
                    _documentDatasetInfoDataFile.Value.Message = $"Kopiowanie archiwum do {destinationLocation}";
                    _documentDatasetInfoDataFile.Save();
                    File.Copy(_archiveFile, Path.Combine(destinationLocation, "Dokumenty.zip"), true);
                }
                catch (Exception e)
                {
                    //Failure during copy is critical.
                    throw new Exception($"Nie udało się skopiować zbioru do {destinationLocation}", e);
                }
            });
        }

        private void DecompressBackup(string password)
        {
            try
            {
                _compressionService.Decompress(_archiveFile, password, _decompressedFolder);
                _documentDatasetInfoDataFile.Load();
                _documentDatasetInfoDataFile.Value.State = DatasetState.Opened;
                _documentDatasetInfoDataFile.Value.Message = string.Empty;
                _documentDatasetInfoDataFile.Save();
            }
            catch(Exception e)
            {
                _documentDatasetInfoDataFile.Load();
                _documentDatasetInfoDataFile.Value.State = DatasetState.OpeningError;
                _documentDatasetInfoDataFile.Value.Message = string.Equals(e.Message, "invalid password", StringComparison.InvariantCultureIgnoreCase) ? "Nieprawidłowe hasło" : e.Message;
                _documentDatasetInfoDataFile.Save();
            }
        }

        private void RemoveSource() => Directory.Delete(_decompressedFolder, true);

    }
}
