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
      
        private readonly ICompressionService _compressionService;
        private readonly DocumentDatasetInfoDataFile _documentDatasetInfoDataFile;
        private readonly DataFile _datasetArchive = new DataFile { FileName = "Dokumenty.zip" };
        
        private readonly List<string> _fileBackupLocations;
        private readonly List<string> _datasetBackupLocations;

        public DocumentDatasetService(
            IConfiguration configuration, 
            ICompressionService compressionService,
            DocumentDatasetInfoDataFile documentDatasetInfoDataFile)
        {
            _basePath = Path.Combine(configuration.GetValue<string>("DatasetPath"), "Dokumenty");
            _datasetArchive.Location = _basePath;

            _fileBackupLocations = configuration.GetSection("DatasetFilesBackups").Get<List<string>>();
            _datasetBackupLocations = configuration.GetSection("DatasetBackups").Get<List<string>>(); 
            
            _compressionService = compressionService;
            _documentDatasetInfoDataFile = documentDatasetInfoDataFile;
        }

        public DocumentDatasetInfo GetInfo()
        {
            try
            {
                _documentDatasetInfoDataFile.Load();
                return _documentDatasetInfoDataFile.Value;
            }
            catch (Exception e)
            {
                return new DocumentDatasetInfo { State = DatasetState.UnknownError, Error = e.Message };
            }
        }

        public DocumentDatasetInfo Open(string password)
        {
            _documentDatasetInfoDataFile.Load();
            if (_documentDatasetInfoDataFile.Value.State != DatasetState.Closed && _documentDatasetInfoDataFile.Value.State != DatasetState.OpeningError)
                return _documentDatasetInfoDataFile.Value;
            _documentDatasetInfoDataFile.Value.State = DatasetState.Opening;
            _documentDatasetInfoDataFile.Value.Error = string.Empty;
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
            _documentDatasetInfoDataFile.Value.Error = string.Empty;
            _documentDatasetInfoDataFile.Save();

            new Thread(() =>
            {
                try
                {
                    if (makeBackups) MakeBackups(password);

                    documentFiles.ForEach(dataFile => File.Delete(dataFile));

                    _documentDatasetInfoDataFile.Load();
                    _documentDatasetInfoDataFile.Value.State = DatasetState.Closed;
                    if (makeBackups)
                        _documentDatasetInfoDataFile.Value.LastCloseDate = DateTime.Now;
                    _documentDatasetInfoDataFile.Value.Error = string.Empty;
                    _documentDatasetInfoDataFile.Save();
                }
                catch (Exception e)
                {
                    _documentDatasetInfoDataFile.Load();
                    _documentDatasetInfoDataFile.Value.State = DatasetState.ClosingError;
                    _documentDatasetInfoDataFile.Value.Error = e.Message;
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
            _fileBackupLocations.ForEach(location =>
            {
                var destinationLocation = Path.Combine(location, "Dokumenty");
                Directory.CreateDirectory(destinationLocation);
                documentFiles.ForEach(dataFile =>
                {
                    var destinationFile = Path.Combine(destinationLocation, Path.GetFileName(dataFile));
                    try
                    {
                        File.Copy(dataFile, destinationFile, true);
                    }
                    catch (Exception e)
                    {
                        //Failure during copy is critical.
                        throw new Exception($"Nie udało się skopiować zbioru do {destinationFile}", e);
                    }
                });
                File.Copy(_documentDatasetInfoDataFile.DataFile.FileNameWithLocation, Path.Combine(location, _documentDatasetInfoDataFile.DataFile.FileName), true);
            });
        }

        private void MakeCompressedBackups(string password)
        {
            File.Delete(_datasetArchive.FileNameWithLocation);
            _compressionService.Compress(documentFiles, _datasetArchive.FileNameWithLocation, password);
            
            _datasetBackupLocations.ForEach(location =>
            {
                try
                {
                    File.Copy(_datasetArchive.FileNameWithLocation, Path.Combine(location, _datasetArchive.FileName), true);
                }
                catch (Exception e)
                {
                    //Failure during copy is critical.
                    throw new Exception($"Nie udało się skopiować zbioru do {location}", e);
                }
            });
        }

        private void DecompressBackup(string password)
        {
            try
            {
                _compressionService.Decompress(_datasetArchive.FileNameWithLocation, password);
                _documentDatasetInfoDataFile.Load();
                _documentDatasetInfoDataFile.Value.State = DatasetState.Opened;
                _documentDatasetInfoDataFile.Value.Error = string.Empty;
                _documentDatasetInfoDataFile.Save();
            }
            catch(Exception e)
            {
                _documentDatasetInfoDataFile.Load();
                _documentDatasetInfoDataFile.Value.State = DatasetState.OpeningError;
                _documentDatasetInfoDataFile.Value.Error = string.Equals(e.Message, "invalid password", StringComparison.InvariantCultureIgnoreCase) ? "Nieprawidłowe hasło" : e.Message;
                _documentDatasetInfoDataFile.Save();
            }
        }

        private List<string> documentFiles => Directory.GetFiles(_datasetArchive.Location).ToList().Where(f => f != _datasetArchive.FileNameWithLocation).ToList();

    }
}
