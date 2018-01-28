using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.Engine.Models
{
    public class DataHandleResult
    {
        public bool Success { get; set; }

        public string Error { get; set; }

        public KeyValuePair<string,string>[] ValuesToAdd { get; set; } = new KeyValuePair<string, string>[0];
    }
}
