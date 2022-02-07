using FinancesApi.DataFiles;
using FinancesApi.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace FinancesApi.Services
{
    public interface IAccountingDatasetService
    {
        AccountingDatasetInfo GetInfo();

        AccountingDatasetInfo Open(string password);

        AccountingDatasetInfo Close(string password, bool makeBackups);
        void Execute();
    }

    public class AccountingDatasetService : IAccountingDatasetService
    {
        private string _basePath;
        private string _archiveFile;
        private string _decompressedFolder;

        private readonly ICompressionService _compressionService;
        private readonly AccountingDatasetInfoDataFile _accountingDatasetInfoDataFile;
        
        private readonly List<string> _fileBackupLocations;
        private readonly List<string> _datasetBackupLocations;

        public AccountingDatasetService(
            IConfiguration configuration, 
            ICompressionService compressionService,
            AccountingDatasetInfoDataFile accountingDatasetInfoDataFile)
        {
            _basePath = Path.Combine(configuration.GetValue<string>("DatasetPath"));;
            _archiveFile = Path.Combine(_basePath, "Księgowość.zip");
            _decompressedFolder = Path.Combine(_basePath, "Księgowość");

            _fileBackupLocations = configuration.GetSection("DatasetFilesBackups").Get<List<string>>();
            _datasetBackupLocations = configuration.GetSection("DatasetBackups").Get<List<string>>(); 
            
            _compressionService = compressionService;
            _accountingDatasetInfoDataFile = accountingDatasetInfoDataFile;
            _accountingDatasetInfoDataFile.Load();
        }

        public AccountingDatasetInfo GetInfo()
        {
            try
            {
                return _accountingDatasetInfoDataFile.Value;
            }
            catch (Exception e)
            {
                return new AccountingDatasetInfo { State = DatasetState.UnknownError, Message = e.Message };
            }
        }

        public AccountingDatasetInfo Open(string password)
        {
            _accountingDatasetInfoDataFile.Load();
            if (_accountingDatasetInfoDataFile.Value.State != DatasetState.Closed && _accountingDatasetInfoDataFile.Value.State != DatasetState.OpeningError)
                return _accountingDatasetInfoDataFile.Value;
            _accountingDatasetInfoDataFile.Value.State = DatasetState.Opening;
            _accountingDatasetInfoDataFile.Value.Message = string.Empty;
            _accountingDatasetInfoDataFile.Save();

            new Thread(() => {
                DecompressBackup(password);
            }).Start();
            return _accountingDatasetInfoDataFile.Value;
        }
            

        public AccountingDatasetInfo Close(string password, bool makeBackups)
        {
            _accountingDatasetInfoDataFile.Load();
            if (_accountingDatasetInfoDataFile.Value.State != DatasetState.Opened && _accountingDatasetInfoDataFile.Value.State != DatasetState.ClosingError)
                return _accountingDatasetInfoDataFile.Value;
            _accountingDatasetInfoDataFile.Value.State = DatasetState.Closing;
            _accountingDatasetInfoDataFile.Value.Message = string.Empty;
            _accountingDatasetInfoDataFile.Save();

            new Thread(() =>
            {
                try
                {
                    if (makeBackups) MakeAllBackups(password);
                    RemoveSource();
                    
                    _accountingDatasetInfoDataFile.Load();
                    _accountingDatasetInfoDataFile.Value.State = DatasetState.Closed;
                    if (makeBackups)
                        _accountingDatasetInfoDataFile.Value.LastCloseDate = DateTime.Now;
                    _accountingDatasetInfoDataFile.Value.Message = string.Empty;
                    _accountingDatasetInfoDataFile.Save();
                }
                catch (Exception e)
                {
                    _accountingDatasetInfoDataFile.Load();
                    _accountingDatasetInfoDataFile.Value.State = DatasetState.ClosingError;
                    _accountingDatasetInfoDataFile.Value.Message = e.Message;
                    _accountingDatasetInfoDataFile.Save();
                }
            }).Start();
            
            return _accountingDatasetInfoDataFile.Value;
        }

        private void MakeAllBackups(string password)
        {
            var mainFolder = Path.Combine(_decompressedFolder, "Uaktualnienie");
            foreach (var file in Directory.GetFiles(mainFolder, "Uaktualnienie*.exe"))
                File.Delete(file);
            MakeBackups();
            MakeBackups(password);
        }

        private void MakeBackups(string password = null)
        {
            _accountingDatasetInfoDataFile.Load();
            _accountingDatasetInfoDataFile.Value.Message = "Tworzenie archiwum";
            _accountingDatasetInfoDataFile.Save();
            
            List<string> destinationLocations;
            if (string.IsNullOrWhiteSpace(password))
                destinationLocations = _fileBackupLocations;
            else
                destinationLocations = _datasetBackupLocations;
            
            _compressionService.Compress(_archiveFile, password, _decompressedFolder);

            destinationLocations.ForEach(destinationLocation =>
            {
                try
                {
                    _accountingDatasetInfoDataFile.Load();
                    _accountingDatasetInfoDataFile.Value.Message = $"Kopiowanie archiwum do {destinationLocation}";
                    _accountingDatasetInfoDataFile.Save();
                    if (string.IsNullOrWhiteSpace(password)) 
                        File.Copy(_archiveFile, Path.Combine(destinationLocation, "Księgowość.zip"), true);
                    else
                        File.Copy(_archiveFile, Path.Combine(destinationLocation, $"Księgowość{DateTime.Now.ToString("yyyy'-'MM'-'dd")}.zip"), true);

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
                _accountingDatasetInfoDataFile.Load();
                _accountingDatasetInfoDataFile.Value.State = DatasetState.Opened;
                _accountingDatasetInfoDataFile.Value.Message = string.Empty;
                _accountingDatasetInfoDataFile.Save();
            }
            catch(Exception e)
            {
                _accountingDatasetInfoDataFile.Load();
                _accountingDatasetInfoDataFile.Value.State = DatasetState.OpeningError;
                _accountingDatasetInfoDataFile.Value.Message = string.Equals(e.Message, "invalid password", StringComparison.InvariantCultureIgnoreCase) ? "Nieprawidłowe hasło" : e.Message;
                _accountingDatasetInfoDataFile.Save();
            }
        }

        private void RemoveSource() => Directory.Delete(_decompressedFolder, true);

        public void Execute()
        {
            _accountingDatasetInfoDataFile.Load();
            if (_accountingDatasetInfoDataFile.Value.State != DatasetState.Opened)
                return;

            Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(_decompressedFolder, "Księga.exe"),
                WorkingDirectory = _decompressedFolder,
                UseShellExecute = true
            });
        }
    }
}
