using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Otus.Teaching.Concurrency.Import.Core.Parsers;
using Otus.Teaching.Concurrency.Import.DataGenerator.Dto;
using Otus.Teaching.Concurrency.Import.Handler.Entities;

namespace Otus.Teaching.Concurrency.Import.DataAccess.Parsers
{
    public class XmlParser
        : IDataParser<List<Customer>>
    {
        private string _dataFilePath;

        public XmlParser(string dataFilePath)
        {
            _dataFilePath = dataFilePath;
        }

        public List<Customer> Parse()
        {
            //Parse data
            //Посмотреть где добавляются ссылки на проекты
            using var stream = File.OpenRead(_dataFilePath);
            CustomersList customersList = (CustomersList)new XmlSerializer(typeof(CustomersList)).Deserialize(stream);

            return customersList.Customers;
        }
    }
}