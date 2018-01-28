using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.Db
{
    public class ClientRepository : IDisposable
    {
        CarDb db = new CarDb();

        public string AddUser(string name, string surname, string phone)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                var prev = db.Client.OrderByDescending(o => o.Id).FirstOrDefault()?.Id.ToString() ?? "00001";
                var pin = prev.Length > 5 ? prev.Substring(0, 5) : prev.PadLeft(5, '0');
                var client = new Client
                {
                    Name = name,
                    Surname = surname,
                    Phone = phone,
                    Pin = pin,
                };
                db.Client.Add(client);
                db.SaveChanges();
                transaction.Commit();
                return pin;
            }

        }

        public bool Exists(string pin)
        {
            pin = pin.Length > 5 ? pin.Substring(0, 5) : pin.PadLeft(5, '0');
            return db.Client.Any(a => a.Pin == pin);
        }

        public void Dispose()
        {
            ((IDisposable)db).Dispose();
        }
    }
}
