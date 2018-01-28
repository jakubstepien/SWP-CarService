using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.Db
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(5)]
        [Index]
        public string Pin { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; }

        [Required]
        [MaxLength(64)]
        public string Surname { get; set; }

        [Required]
        [MaxLength(9)]
        public string Phone { get; set; }
    }
}
