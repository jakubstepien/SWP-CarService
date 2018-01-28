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
            condCode = condCode.Replace("&amp;", "&");
            var engine = new Jint.Engine();
            foreach (var variable in variables)
            {
                engine.SetValue(variable.Key, variable.Value);
            }
            engine.Execute(condCode);
            var result = engine.GetCompletionValue().AsBoolean();
            return result;
        }

        internal string GetAssignScriptResult(string script, string field, Dictionary<string, string> answers)
        {
            var engine = new Jint.Engine();
            foreach (var variable in answers)
            {
                engine.SetValue(variable.Key, variable.Value);
            }
            engine.Execute(script);
            var result = engine.GetCompletionValue();
            return result.IsNull() ? null : result.ToString();
        }

        private void ReassignChangedValues(Jint.Engine engine, Dictionary<string, string> answers)
        {
            foreach (var field in answers.ToArray())
            {
                var val = engine.GetValue(field.Key);
                var newVal = val.IsNull() ? null : val.AsString();
                if (string.IsNullOrEmpty(newVal))
                {
                    answers.Remove(field.Key);
                }
                else if(newVal != field.Value)
                {
                    answers.Remove(field.Key);
                    answers.Add(field.Key, newVal);
                }
            }
        }
    }
}
