using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.Db
{
    public class Car
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(4)]
        public string ProductionYear { get; set; }

        public Client Client { get; set; }

        [ForeignKey(nameof(Client))]
        public int IdClient { get; set; }

        public Brand Brand { get; set; }

        [ForeignKey(nameof(Brand))]
        public int IdBrand { get; set; }
    }
}
