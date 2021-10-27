using FinancesApi.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

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

        public DatasetService(IConfiguration configuration)
        {
            var basePath = configuration.GetValue<string>("DatasetPath");
            var infoFilePath = Path.Combine(basePath, "info.json");
            _datasetInfo = new Jsonfile<DatasetInfo>(infoFilePath);
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
            //try
            //{
                _datasetInfo.Load();
                if (_datasetInfo.Value.State != DatasetState.Closed)
                    return null;
                _datasetInfo.Value.State = DatasetState.Open;
                _datasetInfo.Save();
                return _datasetInfo.Value;
            //}
            //catch (Exception e)
            //{

            //}
        }

        public DatasetInfo Close()
        {
            //try
            //{
            _datasetInfo.Load();
            if (_datasetInfo.Value.State != DatasetState.Open)
                return null;
            _datasetInfo.Value.State = DatasetState.Closed;
            _datasetInfo.Save();
            return _datasetInfo.Value;
            //}
            //catch (Exception e)
            //{

            //}
        }
    }
}
