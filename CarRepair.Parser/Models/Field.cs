using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.Parser.Models
{
    public enum FieldType
    {
        RegularField,
        Record,
    }

    public class Field
    {
        public string Name { get; set; }

        public string Prompt { get; set; }

        public string NoMatchErrorPrompt { get; set; }

        public string GotoVariable { get; set; }

        public FieldType FieldType { get; set; }

        public string FilledJavascript { get; set; }

        public Grammar Grammar { get; set; }
    }
}
