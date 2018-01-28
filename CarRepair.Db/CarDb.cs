using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.Db
{
    class CarDb : DbContext
    {
        public CarDb() : base("CarDb")
        {
        }

        public DbSet<Car> Cars { get; set; }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Client> Client { get; set; }

        public DbSet<Fault> Fault { get; set; }

        public DbSet<Price> Price { get; set; }

        public DbSet<Repair> Repair { get; set; }
    }
}
