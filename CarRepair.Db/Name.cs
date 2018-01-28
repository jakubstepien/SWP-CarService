using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.Db
{
    public class Name
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(64)]
        [Required]
        public string Value { get; set; }
    }
}
