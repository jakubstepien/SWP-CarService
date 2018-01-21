using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Speech.Recognition.SrgsGrammar;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CarRepair.Parser.Utils
{
    abstract class GrammarValidator
    {
        /// <summary>
        /// Probuje zwalidować garmatykę, dodaje atrybuty takie jak xml:lang lub zmienia namespace by był dobrze rozumiany przez ASR
        /// </summary>
        /// <param name="xmlGrammar">zawartość xmla z gramatyką</param>
        /// <returns>Czy wyjściowy xml jest poprawny</returns>
        public static bool ValidateAndFixGrammar(ref string xmlGrammar)
        {
            var node = System.Xml.Linq.XElement.Parse(xmlGrammar);
            var culture = ConfigurationManager.AppSettings["defaultCulture"];
            if (node.Attribute(System.Xml.Linq.XNamespace.Xml + "lang") == null)
            {
                xmlGrammar = xmlGrammar.Replace("<grammar", $"<grammar xml:lang=\"{culture}\"");
            }
            if (string.IsNullOrEmpty(node.Name.Namespace.ToString()))
            {
                xmlGrammar = xmlGrammar.Replace("<grammar", "<grammar xmlns=\"http://www.w3.org/2001/06/grammar\" ");
            }
            else
            {
                xmlGrammar = Regex.Replace(xmlGrammar, "xmlns=\".*?\"", "xmlns=\"http://www.w3.org/2001/06/grammar\" ");
            }
            if(node.Attribute("version") == null)
            {
                xmlGrammar = xmlGrammar.Replace("<grammar", "<grammar version=\"1.0\" ");
            }
            using (var reader = System.Xml.XmlReader.Create(new StringReader(xmlGrammar)))
            {
                try
                {
                    var srgs = new SrgsDocument(reader);
                }
                catch(Exception e)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
