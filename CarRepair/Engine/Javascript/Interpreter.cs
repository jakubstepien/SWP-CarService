using Jint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.Engine.Javascript
{
    class Interpreter
    {
        public string GetGotoValue(string gotoVariableName, string switchCode, Dictionary<string, string> variables)
        {
            var engine = new Jint.Engine();
            foreach (var variable in variables)
            {
                engine.SetValue(variable.Key, variable.Value);
            }
            engine.Execute(switchCode);
            var gotoVal = engine.GetValue(gotoVariableName);
            return gotoVal.AsString();
        }
    }
}
