using Otus.Teaching.Concurrency.Import.DataAccess;
using Otus.Teaching.Concurrency.Import.DataAccess.Repositories;
using Otus.Teaching.Concurrency.Import.Handler.Entities;
using Otus.Teaching.Concurrency.Import.Loader.Loaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Otus.Teaching.Concurrency.Import.Core.Loaders
{
    public class XmlDataLoader
        : IDataLoader
    {
        private List<Customer> _customers;
        private int _countThreads;
        private int _countTries;

        public XmlDataLoader(List<Customer> customers, int countThreads, int countTries)
        {
            _customers = customers;
            _countThreads = countThreads;
            _countTries = countTries;
        }

        public void LoadData()
        {
            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < _countThreads; i++)
            {
                List<Customer> partCustomers = _customers.Where(s => s.Id % _countThreads == i).ToList();

                threads.Add(StartHandlerThread(partCustomers));
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }

        private Thread StartHandlerThread(List<Customer> partCustomers)
        {
            var thread = new Thread(DataLoad);

            thread.Start(partCustomers);

            return thread;
        }

        private void DataLoad(object customers)
        {
            int tryNumber = 0;
            while (tryNumber < _countTries)
            {
                try
                {
                    List<Customer> listCustomers = (List<Customer>)customers;

                    using DataContext dataContext = new DataContext();

                    var customerRepository = new CustomerRepository(dataContext);
                    foreach (var customer in listCustomers)
                    {
                        customerRepository.AddCustomer(customer);
                    }

                    dataContext.SaveChanges();

                    tryNumber = _countTries;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Try number {tryNumber + 1} for ManagedThreadId={Thread.CurrentThread.ManagedThreadId} has fail");
                    Console.WriteLine($"{ex.Message}");
                    tryNumber++;
                }
            }
        }

        public void LoadDataPool()
        {
            WaitHandle[] waitHandles = new WaitHandle[_countThreads];
            for (int i = 0; i < _countThreads; i++)
            {
                CustomersLoadPoolPart item = new CustomersLoadPoolPart()
                {
                    Customers = _customers.Where(s => s.Id % _countThreads == i).ToList()
                };
                waitHandles[i] = item.WaitHandle;

                ThreadPool.QueueUserWorkItem(DataLoadPool, item);
            }

            WaitHandle.WaitAll(waitHandles);
        }

        private void DataLoadPool(object item)
        {
            int tryNumber = 0;
            while (tryNumber < _countTries)
            {
                try
                {

                    CustomersLoadPoolPart itemLoad = (CustomersLoadPoolPart)item;

                    using DataContext dataContext = new DataContext();

                    var customerRepository = new CustomerRepository(dataContext);
                    foreach (var customer in itemLoad.Customers)
                    {
                        customerRepository.AddCustomer(customer);
                    }

                    dataContext.SaveChanges();

                    var autoResetEvent = (AutoResetEvent)itemLoad.WaitHandle;
                    autoResetEvent.Set();

                    tryNumber = _countTries;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Try number {tryNumber + 1} for ManagedThreadId={Thread.CurrentThread.ManagedThreadId} has fail");
                    Console.WriteLine($"{ex.Message}");
                    tryNumber++;
                }
            }
        }
    }
}