using System;
using System.Threading.Tasks;
using Otus.Teaching.Concurrency.Import.Handler.Entities;
using Otus.Teaching.Concurrency.Import.Handler.Repositories;

namespace Otus.Teaching.Concurrency.Import.DataAccess.Repositories
{
    public class CustomerRepository
        : ICustomerRepository
    {
        private DataContext _dataContext;

        public CustomerRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void AddCustomer(Customer customer)
        {
            _dataContext.Customers.Add(customer);
            //Console.WriteLine($"Add customer: {customer}");
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            return await _dataContext.Customers.FindAsync(id);
        }
    }
}