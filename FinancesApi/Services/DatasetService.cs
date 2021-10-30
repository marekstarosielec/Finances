using FinancesApi.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
                _datasetInfo.Value.State = DatasetState.Opening;
                _datasetInfo.Save();
            Unpack();
            return _datasetInfo.Value;
            //}
            //catch (Exception e)
            //{

            //}
        }
            
        public void Unpack()
        {
            Thread t = new Thread(() => {
                //call stored procedure which will run longer time since it calls another remote stored procedure and
                //waits until it's done processing
                Thread.Sleep(10000);
                _datasetInfo.Load();
                _datasetInfo.Value.State = DatasetState.Open;
                _datasetInfo.Save();
            });
            t.Start();
        }


        public void Pack()
        {
            Thread t = new Thread(() => {
                //call stored procedure which will run longer time since it calls another remote stored procedure and
                //waits until it's done processing
                Thread.Sleep(10000);
                _datasetInfo.Load();
                _datasetInfo.Value.State = DatasetState.Closed;
                _datasetInfo.Save();
            });
            t.Start();
        }

        public DatasetInfo Close()
        {
            //try
            //{
            _datasetInfo.Load();
            if (_datasetInfo.Value.State != DatasetState.Open)
                return null;
            _datasetInfo.Value.State = DatasetState.Closing;
            _datasetInfo.Save();
            Pack();
            return _datasetInfo.Value;
            //}
            //catch (Exception e)
            //{

            //}
        }
    }
}
