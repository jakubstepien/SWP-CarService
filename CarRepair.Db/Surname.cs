using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.Db
{
    class Surname
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(64)]
        [Required]
        public string Value { get; set; }
    }
}
