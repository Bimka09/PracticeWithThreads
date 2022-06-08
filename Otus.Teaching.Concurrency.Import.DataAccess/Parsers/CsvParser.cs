using Otus.Teaching.Concurrency.Import.Core.Parsers;
using Otus.Teaching.Concurrency.Import.Core.Serializers;
using Otus.Teaching.Concurrency.Import.DataGenerator.Dto;
using Otus.Teaching.Concurrency.Import.Handler.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Otus.Teaching.Concurrency.Import.DataAccess.Parsers
{
    public class CsvParser
        : IDataParser<List<Customer>>
    {
        private string _dataFilePath;

        public CsvParser(string dataFilePath)
        {
            _dataFilePath = dataFilePath;
        }

        public List<Customer> Parse()
        {
            CustomersList customersList = new CustomersList();
            customersList.Customers = new List<Customer>();

            using var fileStream = File.OpenRead(_dataFilePath);
            using var streamReader = new StreamReader(fileStream);

            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                customersList.Customers.Add(CsvSerializer.Deserialize<Customer>(line));
            }

            return customersList.Customers;
        }
    }
}
