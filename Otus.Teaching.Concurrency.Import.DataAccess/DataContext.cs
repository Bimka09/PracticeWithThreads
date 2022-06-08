using Microsoft.EntityFrameworkCore;
using Otus.Teaching.Concurrency.Import.Handler.Entities;

namespace Otus.Teaching.Concurrency.Import.DataAccess
{
    public class DataContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public DataContext()
        {
            Database.EnsureCreated();
        }

        public void ClearDb()
        {
            Database.ExecuteSqlRaw("TRUNCATE TABLE public.\"Customers\"");
            SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Username=postgres;Password=k1t2i3f4;Database=CustomersDB;Port=5432");
        }
    }
}
