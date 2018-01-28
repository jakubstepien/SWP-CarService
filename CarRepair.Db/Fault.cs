using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRepair.Db
{
    public class Fault
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        [Index(IsUnique = true)]
        public string Name { get; set; }
    }
}
