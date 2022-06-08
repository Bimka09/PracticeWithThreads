using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Otus.Teaching.Concurrency.Import.Core.Loaders;
using Otus.Teaching.Concurrency.Import.DataGenerator.Generators;
using System.Collections.Generic;
using Otus.Teaching.Concurrency.Import.Handler.Entities;
using Otus.Teaching.Concurrency.Import.DataAccess;
using Otus.Teaching.Concurrency.Import.DataAccess.Parsers;

namespace Otus.Teaching.Concurrency.Import.Loader
{
    class Program
    {
        private static string _dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "customers.xml");
        
        static void Main(string[] args)
        {
            if (args != null && args.Length == 1)
            {
                _dataFilePath = args[0];
            }

            bool isXmlGeneratorAsProcess = ConfigurationManager.AppSettings["IsXmlGeneratorAsProcess"] == "True";
            string xmlGeneratorFullPath = ConfigurationManager.AppSettings["XmlGeneratorFullPath"];
            int countData = int.Parse(ConfigurationManager.AppSettings["CountData"]?.ToString() ?? "1000");
            string dataType = ConfigurationManager.AppSettings["DataType"];
            bool isLoadDataPool = ConfigurationManager.AppSettings["IsLoadDataPool"] == "True";
            int countThreadsLoading = int.Parse(ConfigurationManager.AppSettings["CountThreadsLoading"]?.ToString() ?? "10");
            int countTriesLoading = int.Parse(ConfigurationManager.AppSettings["CountTriesLoading"]?.ToString() ?? "5");

            if (isXmlGeneratorAsProcess && string.IsNullOrEmpty(xmlGeneratorFullPath))
            {
                Console.WriteLine($"Application file name for generating data is required");
                return;
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (!isXmlGeneratorAsProcess)
            {
                Console.WriteLine($"Generating data with process Id {Process.GetCurrentProcess().Id}...");

                GenerateCustomersDataFile(countData, dataType);
            }
            else
            {
                GenerateCustomersDataFileInProcess(xmlGeneratorFullPath, countData, dataType);
            }

            stopwatch.Stop();

            Console.WriteLine($"Generated data. Total time: {stopwatch.ElapsedMilliseconds}ms");

            using var dataContext = new DataContext();
            dataContext.ClearDb();

            stopwatch.Start();

            Console.WriteLine("Parsing data...");

            List<Customer> customers = new List<Customer>();
            if (dataType == "xml")
            {
                var xmlParser = new XmlParser(_dataFilePath);
                customers = xmlParser.Parse();
            }
            else if (dataType == "csv")
            {
                var csvParser = new CsvParser(_dataFilePath);
                customers = csvParser.Parse();
            }

            stopwatch.Stop();

            Console.WriteLine($"Parsed data. Total time: {stopwatch.ElapsedMilliseconds}ms");

            stopwatch.Start();

            Console.WriteLine("Loading data...");

            var loader = new XmlDataLoader(customers, countThreadsLoading, countTriesLoading);

            if (isLoadDataPool)
            {
                loader.LoadDataPool();
            }
            else
            {
                loader.LoadData();
            }

            stopwatch.Stop();

            Console.WriteLine($"Loaded data. Total time: {stopwatch.ElapsedMilliseconds}ms");

            Console.ReadKey();

        }
        static void GenerateCustomersDataFile(int countData, string dataType)
        {
            if (dataType == "xml")
            {
                var xmlGenerator = new XmlGenerator(_dataFilePath, countData);
                xmlGenerator.Generate();
            }
            else if (dataType == "csv")
            {
                var csvGenerator = new CsvGenerator(_dataFilePath, countData);
                csvGenerator.Generate();
            }
        }
        static void GenerateCustomersDataFileInProcess(string fileName, int countData, string dataType)
        {
            var proccess = StartHandlerProcess(fileName, new string[] { _dataFilePath.Substring(0, _dataFilePath.LastIndexOf(".")), countData.ToString(), dataType });

            Console.WriteLine($"Generating data started with process Id {proccess.Id}...");

            proccess.WaitForExit();
        }

        static Process StartHandlerProcess(string fileName, string[] args)
        {
            var startInfo = new ProcessStartInfo()
            {
                FileName = fileName
            };

            foreach (var arg in args)
            {
                startInfo.ArgumentList.Add(arg);
            }

            var process = Process.Start(startInfo);

            return process;
        }
    }
}