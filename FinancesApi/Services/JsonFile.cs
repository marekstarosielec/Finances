using FinancesApi.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace FinancesApi.Services
{

    public class Jsonfile<T>
    {
        private static readonly Object obj = new Object();

        public DataFile DataFile { get; }

        public T Value { get; private set; }

        public Jsonfile(IConfiguration configuration, string fileName) 
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException($"'{nameof(fileName)}' cannot be null or whitespace.", nameof(fileName));

            var basePath = configuration.GetValue<string>("DatasetPath");
            DataFile = new DataFile
            {
                Location = basePath,
                FileName = fileName
            };
        }

        public void Load()
        {
            if (string.IsNullOrWhiteSpace(DataFile.FileName) || !File.Exists(DataFile.FileNameWithLocation))
                throw new FileNotFoundException();
            string jsonString = File.ReadAllText(DataFile.FileNameWithLocation, Encoding.Latin1);
            Value = JsonSerializer.Deserialize<T>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        public void Save()
        {
            lock (obj)
            {
                var serializedValue = JsonSerializer.Serialize(Value);
                File.WriteAllText(DataFile.FileNameWithLocation, serializedValue);
            }
        }
    }
}
