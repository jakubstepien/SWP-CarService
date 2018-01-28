using CarRepair.Db;
using System.Text.RegularExpressions;
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
            FieldGrammarType.Number.LoadGrammar();

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
                Id = formElement.GetAttributeByName("id")?.Value
            };
            form.Fields = formElement.Elements()
                .Select(s =>
                {
                    switch (s.Name.LocalName.ToLower())
                    {
                        case "field":
                            return ParseField(s);
                        case "var":
                            return ParseVar(s);
                        case "block":
                            return ParseBlock(s);
                        default:
                            return null;
                    }
                })
                .Where(w => w != null)
                .ToArray();
            return form;
        }

        private Field ParseBlock(XElement fieldElement)
        {
            var field = new Field
            {
                Name = fieldElement.GetAttributeByName("name")?.Value?.Trim(),
                FieldType = FieldType.Block,
                Prompt = ParsePrompt(fieldElement),
                Cond = fieldElement.GetAttributeByName("cond")?.Value?.Trim(),
            };
            return field;
        }

        private Field ParseVar(XElement fieldElement)
        {
            return new Field
            {
                FieldType = FieldType.Var,
                Name = fieldElement.GetAttributeByName("name")?.Value?.Trim(),
            };
        }

        private Field ParseField(XElement fieldElement)
        {
            var field = new Field
            {
                Name = fieldElement.GetAttributeByName("name")?.Value?.Trim(),
                FieldType = FieldType.RegularField,
                NoMatchErrorPrompt = fieldElement.GetElementByName("catch")?.Value?.Trim(),
                Prompt = ParsePrompt(fieldElement),
                Cond = fieldElement.GetAttributeByName("cond")?.Value?.Trim(),
                FilledIfs = ParseFilledIfs(fieldElement),
            };
            var filledElement = fieldElement.GetElementByName("filled");
            var grammar = fieldElement.GetElementByName("grammar");
            var type = fieldElement.GetAttributeByName("type")?.Value?.Trim().ToFieldGrammarType();
            field.Grammar = ParseGrammar(grammar, type);
            return field;
        }

        private FilledIfElement[] ParseFilledIfs(XElement field)
        {
            var filled = field.GetElementByName("filled");
            if (filled is null)
                return new FilledIfElement[0];

            var ifs = new List<FilledIfElement>();
            foreach (var item in filled.GetElementsByName("if"))
            {
                var type = item.GetElementByName("exit") != null ? FilledIfType.ExitIf : FilledIfType.Other;
                string expr = "";
                var name = "";
                var assign = item.GetElementByName("assign");
                if (assign != null)
                {
                    type = FilledIfType.Assign;
                    expr = assign.GetAttributeByName("expr").Value;
                    name = assign.GetAttributeByName("name").Value;
                }
                var element = new FilledIfElement
                {
                    Type = type,
                    Cond = item.GetAttributeByName("cond").Value,
                    Prompt = ParsePrompt(item),
                    Expr = expr,
                    Name = name,
                };
                ifs.Add(element);
            }
            return ifs.ToArray();
        }

        private static string ParsePrompt(XElement element)
        {
            const string valRegex = "<value\\s*expr=\"(.*?)\"\\s*\\/>";
            var prompt = element.GetElementByName("prompt");
            var promptText = prompt?.Value?.Trim();
            //jedyny element jaki będzie mieć to raczej val
            if (prompt.Nodes().Any(a => a.NodeType == System.Xml.XmlNodeType.Element))
            {
                var actualText = string.Concat(prompt.Nodes()).Trim();
                promptText = Regex.Replace(actualText, valRegex, m => "@@" + m.Groups[1] + "@@");
            }

            return promptText;
        }

        private Grammar ParseGrammar(XElement grammarElement, FieldGrammarType? type)
        {
            if (type.HasValue)
                return type.Value.LoadGrammar();

            var src = grammarElement.GetAttributeByName("src");
            if (src != null && src.Value.StartsWith("db"))
            {
                return LoadExternalGramar(src.Value);
            }

            var grammarXml = grammarElement.ToString();
            var isValid = GrammarValidator.ValidateAndFixGrammar(ref grammarXml);
            if (!isValid)
            {
                throw new ArgumentException("Grammar is invalid - " + grammarElement.ToString());
            }
            var rules = grammarElement.GetElementsByName("rule").ToArray();
            var grammar = new Grammar { Rules = new Rule[rules.Length], XMLGrammar = grammarXml };
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

        private Grammar LoadExternalGramar(string value)
        {
            //przycięcie db#
            value = value.Substring(3, value.Length - 3);
            string[] options = null;
            using (var repo = new GramarRepository())
            {
                switch (value)
                {
                    case "brands":
                        options = repo.GetBrandOptions();
                        break;
                    case "name":
                        options = repo.GetNameOptions();
                        break;
                    case "surname":
                        options = repo.GetSurnameOptions();
                        break;
                    default:
                        break;
                }
            }
            var sb = new StringBuilder("<grammar root=\"top\"><rule id=\"top\">");
            sb.AppendLine("<one-of>");
            foreach (var option in options)
            {
                sb.AppendLine("<item>" + option + "</item>");
            }
            sb.AppendLine("</one-of>");
            sb.AppendLine("</rule></grammar>");
            var xml = sb.ToString();
            var ok = GrammarValidator.ValidateAndFixGrammar(ref xml);
            var grammar = new Grammar
            {
                Rules = new[] { new Rule { Items = new[] { new OneOfRule { Items = options.Select(s => new RuleItem { Content = s }).ToArray() } } } },
                XMLGrammar = xml,
            };
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

        private Form ParseMixedInitativeForm(XElement formElement)
        {
            throw new NotImplementedException();
        }
    }
}
