using CarRepair.Parser.Models;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using CarRepair.Parser.Utils;

namespace CarRepair.Parser
{
    public class VXMLParser
    {
        public Form[] Parse(string src)
        {
            XDocument document = XDocument.Load(src);
            if (document.Root.Name.LocalName.ToLower() != "vxml")
            {
                throw new ArgumentException();
            }
            var forms = document.Root.GetElementsByName("form").ToArray();
            var parsedForms = new Form[forms.Length];
            for (int i = 0; i < forms.Length; i++)
            {
                parsedForms[i] = ParseForm(forms[i]);
            }
            return parsedForms;
        }

        private Form ParseForm(XElement formElement)
        {
            var isMixed = formElement.GetElementsByName("initial").Any();
            if (isMixed)
                return ParseMixedInitativeForm(formElement);

            var fieldTypes = new string[] { "field", "record" };
            var form = new Form()
            {
                Id = formElement.GetAttributeByName("id").Value
            };
            form.Fields = formElement.Elements()
                .Select(s =>
                {
                    switch (s.Name.LocalName.ToLower())
                    {
                        case "field":
                            return ParseField(s);
                        case "record":
                            return ParseRecord(s);
                        default:
                            return null;
                    }
                })
                .Where(w => w != null)
                .ToArray();
            return form;
        }

        private Field ParseField(XElement fieldElement)
        {
            var field = new Field
            {
                Name = fieldElement.GetAttributeByName("name")?.Value?.Trim(),
                FieldType = FieldType.RegularField,
                NoMatchErrorPrompt = fieldElement.GetElementByName("catch")?.Value?.Trim(),
                Prompt = fieldElement.GetElementByName("prompt")?.Value?.Trim(),

            };
            var filledElement = fieldElement.GetElementByName("filled");
            field.FilledJavascript = filledElement?.GetElementByName("script")?.Value?.Trim();
            field.GotoVariable = filledElement?.GetElementByName("goto")?.GetAttributeByName("expr")?.Value?.Trim();
            var grammar = fieldElement.GetElementByName("grammar");
            field.Grammar = ParseGrammar(grammar);
            return field;
        }

        private Grammar ParseGrammar(XElement grammarElement)
        {
            var rules = grammarElement.GetElementsByName("rule").ToArray();
            var grammar = new Grammar { Rules = new Rule[rules.Length], };
            for (int i = 0; i < rules.Length; i++)
            {
                var ruleItems = rules[i].Elements().ToArray();
                var r = new Rule
                {
                    Id = rules[i].GetAttributeByName("id")?.Value,
                    Items = new RuleItem[ruleItems.Length],
                };
                for (int j = 0; j < ruleItems.Length; j++)
                {
                    if (ruleItems[j].IsNamed("one-of"))
                    {
                        r.Items[j] = ParseOneOfGrammar(ruleItems[j]);
                    }
                    else if (ruleItems[j].IsNamed("item"))
                    {
                        r.Items[j] = new RuleItem { Content = ruleItems[j].Value.Trim() };
                    }
                }
                grammar.Rules[i] = r;
            }
            return grammar;
        }

        private RuleItem ParseOneOfGrammar(XElement xElement)
        {
            var items = xElement.GetElementsByName("item").Select(s => new RuleItem { Content = s.Value.Trim() });
            return new OneOfRule
            {
                Content = string.Join(";", items),
                Items = items.ToArray()
            };
        }

        private Field ParseRecord(XElement fieldElement)
        {
            throw new NotImplementedException();
        }

        private Form ParseMixedInitativeForm(XElement formElement)
        {
            throw new NotImplementedException();
        }
    }
}
