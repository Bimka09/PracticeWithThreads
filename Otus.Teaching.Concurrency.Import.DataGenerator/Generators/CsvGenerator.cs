using System.IO;
using Otus.Teaching.Concurrency.Import.Core.Serializers;
using Otus.Teaching.Concurrency.Import.DataGenerator.Dto;
using Otus.Teaching.Concurrency.Import.Handler.Data;
using Otus.Teaching.Concurrency.Import.Handler.Entities;

namespace Otus.Teaching.Concurrency.Import.DataGenerator.Generators
{
    public class CsvGenerator : IDataGenerator
    {
        private readonly string _fileName;
        private readonly int _dataCount;

        public CsvGenerator(string fileName, int dataCount)
        {
            _fileName = fileName;
            _dataCount = dataCount;
        }

        public void Generate()
        {
            var customers = RandomCustomerGenerator.Generate(_dataCount);
            using var fileStream = File.Create(_fileName);
            using var streamWriter = new StreamWriter(fileStream);
            foreach (var customer in customers)
            {
                streamWriter.WriteLine(CsvSerializer.Serialize<Customer>(customer));
            }
        }
    }
}
