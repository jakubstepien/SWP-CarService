using Jint.Parser.Ast;
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
        public bool GetCondResult(string condCode, Dictionary<string, string> variables)
        {
            var engine = new Jint.Engine();
            foreach (var variable in variables)
            {
                engine.SetValue(variable.Key, variable.Value);
            }
            engine.Execute(condCode);
            var result = engine.GetCompletionValue().AsBoolean();
            return result;
        }
    }
}
