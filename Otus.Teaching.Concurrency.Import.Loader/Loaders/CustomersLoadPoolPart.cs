using Otus.Teaching.Concurrency.Import.Handler.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Otus.Teaching.Concurrency.Import.Loader.Loaders
{

    public class CustomersLoadPoolPart
    {
        public List<Customer> Customers { get; set; }
        public WaitHandle WaitHandle { get; private set; }

        public CustomersLoadPoolPart()
        {
            WaitHandle = new AutoResetEvent(false);
        }
    }

}
