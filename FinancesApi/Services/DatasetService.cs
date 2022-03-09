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
    public interface IDatasetService
    {
        DatasetInfo GetInfo();

        DatasetInfo Open(string password);

        DatasetInfo Close(string password);

    }

    public class DatasetService : IDatasetService
    {
        private string _basePath;
        private readonly IConfiguration _configuration;
        private readonly ICompressionService _compressionService;
        private readonly IAccountingDatasetService _accountingDatasetService;
        private readonly IFileService _fileService;
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
            IAccountingDatasetService accountingDatasetService,
            TransactionsDataFile transactionsFile,
            TransactionAccountsDataFile transactionAccountsDataFile,
            TransactionCategoriesDataFile transactionCategoriesDataFile,
            TransactionAutoCategoriesDataFile transactionAutoCategoriesDataFile,
            IFileService fileService,
            SkodaDataFile skodaDataFile,
            MazdaDataFile mazdaDataFile,
            ElectricityDataFile electricityDataFile,
            GasDataFile gasDataFile,
            BalancesDataFile balancesDataFile,
            DatasetInfoDataFile datasetInfoDataFile,
            CurrenciesDataFile currenciesDataFile,
            DocumentsDataFile documentsDataFile,
            CurrencyExchangeDataFile currencyExchangeData,
            TutoringListDataFile tutoringListDataFile,
            TutoringDataFile tutoringDataFile,
            CaseListDataFile caseListDataFile)
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
            _dataFiles.Add(skodaDataFile.DataFile);
            _dataFiles.Add(mazdaDataFile.DataFile);
            _dataFiles.Add(electricityDataFile.DataFile);
            _dataFiles.Add(gasDataFile.DataFile);
            _dataFiles.Add(currencyExchangeData.DataFile);
            _dataFiles.Add(tutoringListDataFile.DataFile);
            _dataFiles.Add(tutoringDataFile.DataFile);
            _dataFiles.Add(caseListDataFile.DataFile);

            _fileBackupLocations = configuration.GetSection("DatasetFilesBackups").Get<List<string>>();
            _datasetBackupLocations = configuration.GetSection("DatasetBackups").Get<List<string>>();
            _configuration = configuration;
            _compressionService = compressionService;
            _accountingDatasetService = accountingDatasetService;
            _fileService = fileService;
            _datasetInfoDataFile = datasetInfoDataFile;
        }

        public DatasetInfo GetInfo()
        {
            try
            {
                var datasetInfoDataFile = new DatasetInfoDataFile(_configuration);
                datasetInfoDataFile.Load();
                return datasetInfoDataFile.Value;
            }
            catch (Exception e)
            {
                return new DatasetInfo { State = DatasetState.UnknownError, Message = e.Message };
            }
        }

        public DatasetInfo Open(string password)
        {
            _datasetInfoDataFile.Load();
            if (_datasetInfoDataFile.Value.State != DatasetState.Closed && _datasetInfoDataFile.Value.State != DatasetState.OpeningError)
                return _datasetInfoDataFile.Value;
            _datasetInfoDataFile.Value.State = DatasetState.Opening;
            _datasetInfoDataFile.Value.Message = string.Empty;
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
            _datasetInfoDataFile.Value.Message = string.Empty;
            _datasetInfoDataFile.Save();
            new Thread(() =>
            {
                _datasetInfoDataFile.Load();
                _datasetInfoDataFile.Value.Message = "Zamykanie księgowości";
                _datasetInfoDataFile.Save(); 
                _accountingDatasetService.Close(password, true, false);
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
                _datasetInfoDataFile.Value.Message = string.Empty;
                _datasetInfoDataFile.Save();
            }
            catch (Exception e)
            {
                _datasetInfoDataFile.Load();
                _datasetInfoDataFile.Value.State = DatasetState.ClosingError;
                _datasetInfoDataFile.Value.Message = e.Message;
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
                        _datasetInfoDataFile.Load();
                        _datasetInfoDataFile.Value.Message = $"Kopiowanie {dataFile.FileName} do {location}";
                        _datasetInfoDataFile.Save();
                        File.Copy(dataFile.FileNameWithLocation, Path.Combine(location, dataFile.FileName), true);
                    }
                    catch (Exception e)
                    {
                        //Failure during copy is critical.
                        throw new Exception($"Nie udało się skopiować zbioru do {Path.Combine(location, dataFile.FileName)}", e);
                    }
                });
                File.Copy(_datasetInfoDataFile.DataFile.FileNameWithLocation, Path.Combine(location, _datasetInfoDataFile.DataFile.FileName), true);
            });
            _fileService.MakeNonCompressedBackups(file => {
                _datasetInfoDataFile.Load();
                _datasetInfoDataFile.Value.Message = $"Kopiowanie dokumentu do {file}";
                _datasetInfoDataFile.Save();
            });
        }

        private void MakeCompressedBackups(string password)
        {
            _datasetInfoDataFile.Load();
            _datasetInfoDataFile.Value.Message = "Tworzenie archiwum";
            _datasetInfoDataFile.Save();
            _compressionService.Compress(_dataFiles.Select(df => df.FileNameWithLocation).ToList(), _datasetArchive.FileNameWithLocation, password);
            var _datasetTodayArchive = new DataFile { FileName = $"Finanse{DateTime.Now.ToString("yyyy'-'MM'-'dd")}.zip", Location = _basePath };

            File.Copy(_datasetArchive.FileNameWithLocation, _datasetTodayArchive.FileNameWithLocation, true);
            _datasetBackupLocations.ForEach(location =>
            {
                try
                {
                    _datasetInfoDataFile.Load();
                    _datasetInfoDataFile.Value.Message = $"Kopiowanie {_datasetTodayArchive.FileName} do {location}";
                    _datasetInfoDataFile.Save();
                    File.Copy(_datasetTodayArchive.FileNameWithLocation, Path.Combine(location, _datasetTodayArchive.FileName), true);
                }
                catch (Exception e)
                {
                    //Failure during copy is critical.
                    throw new Exception($"Nie udało się skopiować zbioru do {location}", e);
                }
            });
            File.Delete(_datasetTodayArchive.FileNameWithLocation);
            _fileService.MakeCompressedBackups(password,
                file => {
                    _datasetInfoDataFile.Load();
                    _datasetInfoDataFile.Value.Message = $"Tworzenie archiwum {file}";
                    _datasetInfoDataFile.Save();
                },
                file =>
                {
                    _datasetInfoDataFile.Load();
                    _datasetInfoDataFile.Value.Message = $"Kopiowanie dokumentu do {file}";
                    _datasetInfoDataFile.Save();
                });
        }

        private void DecompressBackup(string password)
        {
            try
            {
                _datasetInfoDataFile.Load();
                _datasetInfoDataFile.Value.Message = "Rozpakowywanie archiwum";
                _datasetInfoDataFile.Save();
                _compressionService.Decompress(_datasetArchive.FileNameWithLocation, password);
                File.Delete(_datasetArchive.FileNameWithLocation);
                _datasetInfoDataFile.Load();
                _datasetInfoDataFile.Value.State = DatasetState.Opened;
                _datasetInfoDataFile.Value.Message = string.Empty;
                _datasetInfoDataFile.Save();
            }
            catch(Exception e)
            {
                _datasetInfoDataFile.Load();
                _datasetInfoDataFile.Value.State = DatasetState.OpeningError;
                _datasetInfoDataFile.Value.Message = string.Equals(e.Message, "invalid password", StringComparison.InvariantCultureIgnoreCase) ? "Nieprawidłowe hasło" : e.Message;
                _datasetInfoDataFile.Save();
            }
        }
    }
}
