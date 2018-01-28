using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.Db
{
    public class RepairRepository : IDisposable
    {
        CarDb db = new CarDb();

        public bool AddRepair(string userPin, string carBrand, string carProductionYear, string faultName, string description, DateTime date, decimal price)
        {
            using(var transaction = db.Database.BeginTransaction())
            {
                var user = db.Client.AsNoTracking().SingleOrDefault(s => s.Pin == userPin);
                var brand = db.Brands.AsNoTracking().SingleOrDefault(s => s.BrandName == carBrand);
                var fault = db.Fault.AsNoTracking().SingleOrDefault(s => s.Name == faultName);
                var car = new Car
                {
                    IdClient = user.Id,
                    IdBrand = brand.Id,
                    ProductionYear = carProductionYear
                };
                var repiar = new Repair
                {
                    Car = car,
                    IdClient = user.Id,
                    IdFault = fault.Id,
                    RepairDate = date,
                    Description = description,
                    Price = price,
                };
                db.Repair.Add(repiar);
                db.SaveChanges();
                transaction.Commit();
                return true;
            }
           
        }

        public DateTime GetNextPossibleDate()
        {
            return db.Repair.OrderByDescending(o => o.RepairDate).FirstOrDefault()?.RepairDate ?? DateTime.Now.AddDays(1);
        }

        public decimal GetPrice(string fault, string brand)
        {
            return db.Price.FirstOrDefault(f => f.Fault.Name == fault && f.Brand.BrandName == brand).Value;
        }

        public void Dispose()
        {
            ((IDisposable)db).Dispose();
        }
    }
}
