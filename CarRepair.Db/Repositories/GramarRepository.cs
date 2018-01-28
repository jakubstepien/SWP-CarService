using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.Db
{
    public class GramarRepository : IDisposable
    {
        CarDb db = new CarDb();

        public string[] GetBrandOptions()
        {
            return db.Brands.Select(s => s.BrandName).OrderBy(o => o).ToArray();
        }

        public string[] GetNameOptions()
        {
            return db.Name.Select(s => s.Value).OrderBy(o => o).ToArray();
        }

        public string[] GetSurnameOptions()
        {
            return db.Surname.Select(s => s.Value).OrderBy(o => o).ToArray();
        }

        public void Dispose()
        {
            ((IDisposable)db).Dispose();
        }

    }
}
