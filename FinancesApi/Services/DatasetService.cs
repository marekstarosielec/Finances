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
        private readonly DataFile _infoFile = new DataFile { FileName = "info.json" };
        private readonly Jsonfile<DatasetInfo> _datasetInfo;

        private readonly ICompressionService _compressionService;
        private readonly DataFile _datasetArchive = new DataFile { FileName = "Finanse.zip" };
        private readonly DataFile _datasetTodayArchive;
        private readonly List<string> _fileBackupLocations;
        private readonly List<string> _datasetBackupLocations;
        private readonly List<DataFile> _dataFiles = new List<DataFile>
        {
            new DataFile { FileName = "transactions.json" },
            new DataFile { FileName = "transaction-accounts.json" },
            new DataFile { FileName = "transaction-categories.json" },
            new DataFile { FileName = "balances.json" },
            new DataFile { FileName = "transaction-auto-categories.json" }
        };
        

        public DatasetService(IConfiguration configuration, ICompressionService compressionService)
        {
            var basePath = configuration.GetValue<string>("DatasetPath");
            _infoFile.Location = basePath;

            _dataFiles.ForEach(df => df.Location = basePath);

            _datasetArchive.Location = basePath;
            _datasetTodayArchive = new DataFile { FileName = $"Finanse{DateTime.Now.ToString("yyyy'-'MM'-'dd")}.zip", Location = basePath };

            var test = configuration.GetSection("DatasetFilesBackups").Get<List<string>>();
            _fileBackupLocations = configuration.GetSection("DatasetFilesBackups").Get<List<string>>();
            _datasetBackupLocations = configuration.GetSection("DatasetBackups").Get<List<string>>(); 

            _datasetInfo = new Jsonfile<DatasetInfo>(_infoFile.FileNameWithLocation);
            _compressionService = compressionService;
        }

        public DatasetInfo GetInfo()
        {
            try
            {
                _datasetInfo.Load();
                return _datasetInfo.Value;
            }
            catch (Exception e)
            {
                return new DatasetInfo { State = DatasetState.Error };
            }
        }

        public DatasetInfo Open(string password)
        {
            _datasetInfo.Load();
            if (_datasetInfo.Value.State != DatasetState.Closed)
                return null;
            _datasetInfo.Value.State = DatasetState.Opening;
            _datasetInfo.Save();
            Unpack(password);
            return _datasetInfo.Value;
        }
            
        public void Unpack(string password)
        {
            Thread t = new Thread(() => {
                _compressionService.Decompress(_datasetArchive.FileNameWithLocation, password);
                File.Delete(_datasetArchive.FileNameWithLocation);
                _datasetInfo.Load();
                _datasetInfo.Value.State = DatasetState.Opened;
                _datasetInfo.Save();
            });
            t.Start();
        }


        public void Pack(string password)
        {
            Thread t = new Thread(() => {
                try
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

                            }
                        });
                        File.Copy(_infoFile.FileNameWithLocation, Path.Combine(location, _infoFile.FileName), true);
                    });

                    _compressionService.Compress(_dataFiles.Select(df => df.FileNameWithLocation).ToList(), _datasetArchive.FileNameWithLocation, password);
                    File.Copy(_datasetArchive.FileNameWithLocation, _datasetTodayArchive.FileNameWithLocation);
                    _datasetBackupLocations.ForEach(location =>
                    {
                        try
                        {
                            File.Copy(_datasetTodayArchive.FileNameWithLocation, Path.Combine(location, _datasetTodayArchive.FileName), true);
                        }
                        catch (Exception e)
                        {

                        }
                    });
                    File.Delete(_datasetTodayArchive.FileNameWithLocation);
                    _dataFiles.ForEach(dataFile => File.Delete(dataFile.FileNameWithLocation));
                    _datasetInfo.Load();
                    _datasetInfo.Value.State = DatasetState.Closed;
                    _datasetInfo.Value.LastCloseDate = DateTime.Now;
                    _datasetInfo.Save();
                }
                catch (Exception e)
                {
                    _datasetInfo.Load();
                    _datasetInfo.Value.State = DatasetState.Error;
                    _datasetInfo.Value.Error = e.Message;
                    _datasetInfo.Save();
                }
            });
            t.Start();
        }

        public DatasetInfo Close(string password)
        {
            _datasetInfo.Load();
            if (_datasetInfo.Value.State != DatasetState.Opened)
                return null;
            _datasetInfo.Value.State = DatasetState.Closing;
            _datasetInfo.Save();
            Pack(password);
            return _datasetInfo.Value;
        }
    }
}
