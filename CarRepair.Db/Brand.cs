using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.Db
{
    public class Brand
    {
        public Brand()
        {
            Cars = new HashSet<Car>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        [Index(IsUnique = true)]
        public string BrandName { get; set; }

        public ICollection<Car> Cars { get; set; }
    }
}
