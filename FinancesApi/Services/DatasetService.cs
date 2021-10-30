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

        DatasetInfo Open();

        DatasetInfo Close();
    }

    public class DatasetService : IDatasetService
    {
        private readonly Jsonfile<DatasetInfo> _datasetInfo;
        private readonly ICompressionService _compressionService;
        private readonly string _datasetArchive;
        private readonly List<string> _dataFiles = new List<string>
        {
            "transactions.json"
        };

        public DatasetService(IConfiguration configuration, ICompressionService compressionService)
        {
            var basePath = configuration.GetValue<string>("DatasetPath");
            var infoFilePath = Path.Combine(basePath, "info.json");
            _dataFiles = _dataFiles.Select(dataFile => Path.Combine(basePath, dataFile)).ToList();
            _datasetArchive = Path.Combine(basePath, "dataset.zip");

            _datasetInfo = new Jsonfile<DatasetInfo>(infoFilePath);
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

        public DatasetInfo Open()
        {
            _datasetInfo.Load();
            if (_datasetInfo.Value.State != DatasetState.Closed)
                return null;
            _datasetInfo.Value.State = DatasetState.Opening;
            _datasetInfo.Save();
            Unpack();
            return _datasetInfo.Value;
        }
            
        public void Unpack()
        {
            Thread t = new Thread(() => {
                _compressionService.Decompress(_datasetArchive);
                _datasetInfo.Load();
                _datasetInfo.Value.State = DatasetState.Open;
                _datasetInfo.Save();
            });
            t.Start();
        }


        public void Pack()
        {
            Thread t = new Thread(() => {
                _compressionService.Compress(_dataFiles, _datasetArchive);
                _datasetInfo.Load();
                _datasetInfo.Value.State = DatasetState.Closed;
                _datasetInfo.Save();
            });
            t.Start();
        }

        public DatasetInfo Close()
        {
            _datasetInfo.Load();
            if (_datasetInfo.Value.State != DatasetState.Open)
                return null;
            _datasetInfo.Value.State = DatasetState.Closing;
            _datasetInfo.Save();
            Pack();
            return _datasetInfo.Value;
        }
    }
}
