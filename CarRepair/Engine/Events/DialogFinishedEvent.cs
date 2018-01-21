using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.Engine.Events
{
    public class DialogFinishedEvent : EventArgs
    {
        public string Prompt { get; set; }
    }
}
