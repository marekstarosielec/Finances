﻿using FinancesApi.DataFiles;
using FinancesApi.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace FinancesApi.Services
{
    public interface IDatasetService
    {
        DatasetInfo GetInfo();

        DatasetInfo Open(string password);

        DatasetInfo Close(string password);
    }

    public class DatasetService : IDatasetService
    {
        private string _basePath;
      
        private readonly ICompressionService _compressionService;
        private readonly DatasetInfoDataFile _datasetInfoDataFile;
        private readonly DataFile _datasetArchive = new DataFile { FileName = "Finanse.zip" };
        private readonly List<string> _fileBackupLocations;
        private readonly List<string> _datasetBackupLocations;
        private readonly List<DataFile> _dataFiles = new List<DataFile>
        {
            new DataFile { FileName = "codziennik.xlsm" }
        };
        
        public DatasetService(
            IConfiguration configuration, 
            ICompressionService compressionService,
            TransactionsDataFile transactionsFile,
            TransactionAccountsDataFile transactionAccountsDataFile,
            TransactionCategoriesDataFile transactionCategoriesDataFile,
            TransactionAutoCategoriesDataFile transactionAutoCategoriesDataFile,
            BalancesDataFile balancesDataFile,
            DatasetInfoDataFile datasetInfoDataFile,
            CurrenciesDataFile currenciesDataFile,
            DocumentsDataFile documentsDataFile)
        {
            _basePath = configuration.GetValue<string>("DatasetPath");
            _datasetArchive.Location = _basePath;
            _dataFiles.ForEach(df => df.Location = _basePath);

            _dataFiles.Add(transactionsFile.DataFile);
            _dataFiles.Add(transactionAccountsDataFile.DataFile);
            _dataFiles.Add(transactionCategoriesDataFile.DataFile);
            _dataFiles.Add(transactionAutoCategoriesDataFile.DataFile);
            _dataFiles.Add(balancesDataFile.DataFile);
            _dataFiles.Add(documentsDataFile.DataFile);
            _dataFiles.Add(currenciesDataFile.DataFile);

            _fileBackupLocations = configuration.GetSection("DatasetFilesBackups").Get<List<string>>();
            _datasetBackupLocations = configuration.GetSection("DatasetBackups").Get<List<string>>(); 

            
            _compressionService = compressionService;
            _datasetInfoDataFile = datasetInfoDataFile;
        }

        public DatasetInfo GetInfo()
        {
            try
            {
                _datasetInfoDataFile.Load();
                return _datasetInfoDataFile.Value;
            }
            catch (Exception e)
            {
                return new DatasetInfo { State = DatasetState.UnknownError, Error = e.Message };
            }
        }

        public DatasetInfo Open(string password)
        {
            _datasetInfoDataFile.Load();
            if (_datasetInfoDataFile.Value.State != DatasetState.Closed && _datasetInfoDataFile.Value.State != DatasetState.OpeningError)
                return _datasetInfoDataFile.Value;
            _datasetInfoDataFile.Value.State = DatasetState.Opening;
            _datasetInfoDataFile.Value.Error = string.Empty;
            _datasetInfoDataFile.Save();

            new Thread(() => {
                DecompressBackup(password);
            }).Start();
            return _datasetInfoDataFile.Value;
        }
            

        public DatasetInfo Close(string password)
        {
            _datasetInfoDataFile.Load();
            if (_datasetInfoDataFile.Value.State != DatasetState.Opened && _datasetInfoDataFile.Value.State != DatasetState.ClosingError)
                return _datasetInfoDataFile.Value;
            _datasetInfoDataFile.Value.State = DatasetState.Closing;
            _datasetInfoDataFile.Value.Error = string.Empty;
            _datasetInfoDataFile.Save();
            new Thread(() =>
            {
                MakeBackups(password);
            }).Start();
            return _datasetInfoDataFile.Value;
        }

        private void MakeBackups(string password)
        {
            try
            {
                MakeNonCompressedBackups();
                MakeCompressedBackups(password);

                _dataFiles.ForEach(dataFile => File.Delete(dataFile.FileNameWithLocation));

                _datasetInfoDataFile.Load();
                _datasetInfoDataFile.Value.State = DatasetState.Closed;
                _datasetInfoDataFile.Value.LastCloseDate = DateTime.Now;
                _datasetInfoDataFile.Value.Error = string.Empty;
                _datasetInfoDataFile.Save();
            }
            catch (Exception e)
            {
                _datasetInfoDataFile.Load();
                _datasetInfoDataFile.Value.State = DatasetState.ClosingError;
                _datasetInfoDataFile.Value.Error = e.Message;
                _datasetInfoDataFile.Save();
            }
        }

        private void MakeNonCompressedBackups()
        {
            _fileBackupLocations.ForEach(location =>
            {
                _dataFiles.ForEach(dataFile => {
                    try
                    {
                        File.Copy(dataFile.FileNameWithLocation, Path.Combine(location, dataFile.FileName), true);
                    }
                    catch (Exception e)
                    {
                        //Failure during copy is critical.
                        throw new Exception($"Nie udało się skopiować zbioru do {dataFile.FileNameWithLocation}", e);
                    }
                });
                File.Copy(_datasetInfoDataFile.DataFile.FileNameWithLocation, Path.Combine(location, _datasetInfoDataFile.DataFile.FileName), true);
            });
        }

        private void MakeCompressedBackups(string password)
        {
            _compressionService.Compress(_dataFiles.Select(df => df.FileNameWithLocation).ToList(), _datasetArchive.FileNameWithLocation, password);
            var _datasetTodayArchive = new DataFile { FileName = $"Finanse{DateTime.Now.ToString("yyyy'-'MM'-'dd")}.zip", Location = _basePath };

            File.Copy(_datasetArchive.FileNameWithLocation, _datasetTodayArchive.FileNameWithLocation, true);
            _datasetBackupLocations.ForEach(location =>
            {
                try
                {
                    File.Copy(_datasetTodayArchive.FileNameWithLocation, Path.Combine(location, _datasetTodayArchive.FileName), true);
                }
                catch (Exception e)
                {
                    //Failure during copy is critical.
                    throw new Exception($"Nie udało się skopiować zbioru do {location}", e);
                }
            });
            File.Delete(_datasetTodayArchive.FileNameWithLocation);
        }

        private void DecompressBackup(string password)
        {
            try
            {
                _compressionService.Decompress(_datasetArchive.FileNameWithLocation, password);
                File.Delete(_datasetArchive.FileNameWithLocation);
                _datasetInfoDataFile.Load();
                _datasetInfoDataFile.Value.State = DatasetState.Opened;
                _datasetInfoDataFile.Value.Error = string.Empty;
                _datasetInfoDataFile.Save();
            }
            catch(Exception e)
            {
                _datasetInfoDataFile.Load();
                _datasetInfoDataFile.Value.State = DatasetState.OpeningError;
                _datasetInfoDataFile.Value.Error = string.Equals(e.Message, "invalid password", StringComparison.InvariantCultureIgnoreCase) ? "Nieprawidłowe hasło" : e.Message;
                _datasetInfoDataFile.Save();
            }
        }
    }
}
