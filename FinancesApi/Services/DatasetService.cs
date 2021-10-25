using FinancesApi.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace FinancesApi.Services
{
    public interface IDatasetService
    {
        DatasetInfo GetInfo();
    }

    public class DatasetService : IDatasetService
    {
        private readonly string infoFilePath;

        public DatasetService(IConfiguration configuration)
        {
            var basePath = configuration.GetValue<string>("DatasetPath");
            infoFilePath = Path.Combine(basePath, "info.json");
        }

        public DatasetInfo GetInfo()
        {
            try
            {
  
            }
            catch(Exception e)
            {

            }
            return new DatasetInfo { State = DatasetState.Error };
        }
    }
}
