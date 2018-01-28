using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.Db
{
    public class Price
    {
        [Key]
        public int Id { get; set; }

        public decimal Value { get; set; }

        public Fault Fault { get; set; }

        [ForeignKey(nameof(Fault))]
        public int IdFault { get; set; }

        public Brand Brand { get; set; }


        [ForeignKey(nameof(Brand))]
        public int IdBrand { get; set; }
    }
}
