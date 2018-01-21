using System.Xml;
using System.IO;
using System.Speech.Recognition.SrgsGrammar;
using System.Speech.Recognition;
using CarRepair.Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepair.Parser.Utils
{
    static class TypeGrammarProvider
    {
        public static Models.Grammar LoadGrammar(this FieldGrammarType type)
        {
            switch (type)
            {
                case FieldGrammarType.Digits:
                    return GetDigitsGramat();
                case FieldGrammarType.Phone:
                    return GetPhoneGrammar();
                case FieldGrammarType.Number:
                    return GetNumberGrammar();
                default:
                    return null;
            }
        }

        private static Models.Grammar GetNumberGrammar()
        {
            var xml = LoadGrammarXml("number.xml");
            var grammar = new Models.Grammar
            {
                Rules = new Rule[] { new Rule { Items = new[] { new RuleItem { Content = "[liczba]" } } } },
                XMLGrammar = xml,
            };
            return grammar;
        }

        private static Models.Grammar GetPhoneGrammar()
        {
            var xml = LoadGrammarXml("phone.xml");
            var grammar = new Models.Grammar
            {
                Rules = new Rule[] { new Rule { Items = new[] { new RuleItem { Content = "[9 cyfrowy numer telefonu]" } } } },
                XMLGrammar = xml,
            };
            return grammar;
        }

        private static Models.Grammar GetDigitsGramat()
        {
            var xml = LoadGrammarXml("digits.xml");
            var grammar = new Models.Grammar
            {
                Rules = new Rule[] { new Rule { Items = new[] { new RuleItem { Content = "[ciąg cyfr]" } } } },
                XMLGrammar = xml,
            };
            return grammar;
        }

        private static string LoadGrammarXml(string file)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + $"/grammars/{file}";
            var grammar = File.ReadAllText(path);
            var culture = System.Configuration.ConfigurationManager.AppSettings["defaultCulture"];
            grammar = grammar.Replace("<grammar", $"<grammar xml:lang=\"{culture}\"");
            try
            {
                using (var reader = XmlReader.Create(new StringReader(grammar)))
                {
                    //tworze srgs i gramatyke tylko by sprawdzić czy xml zostanie czytany
                    var srgs = new SrgsDocument(reader);
                    new System.Speech.Recognition.Grammar(srgs);
                    return grammar;
                }
            }
            catch(Exception e)
            {
                throw new Exception("Bład wczytywania gramtyki " + file);
            }
        }


        public static FieldGrammarType? ToFieldGrammarType(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            switch (text.ToLower())
            {
                case "digits":
                    return FieldGrammarType.Digits;
                case "phone":
                    return FieldGrammarType.Phone;
                case "number":
                    return FieldGrammarType.Number;
                default:
                    return null;
            }
        }
    }

    enum FieldGrammarType { Digits, Phone, Number }
}
