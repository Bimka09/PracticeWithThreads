namespace Otus.Teaching.Concurrency.Import.Handler.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
        public override string ToString()
        {
            return $"Customer: Id={Id}, FullName=\"{FullName}\", Email=\"{Email}\", Phone=\"{Phone}\"";
        }
    }
}