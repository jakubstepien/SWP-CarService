using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.Db
{
    public class Repair
    {
        [Key]
        public int Id { get; set; }

        public Client Client { get; set; }

        [ForeignKey(nameof(Client))]
        public int IdClient { get; set; }

        public Fault Fault { get; set; }

        [ForeignKey(nameof(Fault))]
        public int? IdFault { get; set; }

        public DateTime RepairDate { get; set; }

        public decimal Price { get; set; }

        public Car Car { get; set; }

        [ForeignKey(nameof(Car))]
        public int IdCar { get; set; }

        [MaxLength(256)]
        public string Description { get; set; }
    }
}
