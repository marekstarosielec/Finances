using System;
using System.IO;
using System.Text.Json;

namespace FinancesApi.Services
{

    public class Jsonfile<T>
    {
        private string _fileName;

        public T Value { get; private set; 
        }
        public Jsonfile(string fileName) 
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException($"'{nameof(fileName)}' cannot be null or whitespace.", nameof(fileName));

            _fileName = fileName;
        }

        public void Load()
        {
            if (!File.Exists(_fileName))
                throw new FileNotFoundException();
            string jsonString = File.ReadAllText(_fileName);
            Value = JsonSerializer.Deserialize<T>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public void Save()
        {
            var serializedValue = JsonSerializer.Serialize(Value);
            File.WriteAllText(_fileName, serializedValue);
        }
    }
}
