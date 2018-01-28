using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.Parser.Models
{
    public class FilledIfElement
    {
        public string Cond { get; set; }

        public string Prompt { get; set; }

        public string Expr { get; set; }

        public string Name { get; set; }

        public FilledIfType Type { get; set; }
    }

    public enum FilledIfType { Other, ExitIf, Assign }
}
