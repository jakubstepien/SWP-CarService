using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.Parser.Models
{
    public class OneOfRule : RuleItem
    {
        public RuleItem[] Items { get; set; }
    }
}
