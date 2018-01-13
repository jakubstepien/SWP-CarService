using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CarRepair.Parser.Utils
{
    public static class XElementExtensions
    {
        /// <summary>
        /// Zwraca wszystkie dzieci elementu które mają określoną nazwe
        /// </summary>
        /// <param name="element">Element którego dzieci są przeszukiwane</param>
        /// <param name="name">Nazwa która jest poszukiwana</param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetElementsByName(this XElement element, string name)
        {
            return element.Elements().Where(w => w.IsNamed(name));
        }

        /// <summary>
        /// Zwraca pierwsze dziecko elementu o określonej nazwie
        /// </summary>
        /// <param name="element">Element którego dziecko jest wyszukiwane</param>
        /// <param name="name">Nazwa która jest poszukiwana</param>
        /// <returns>Dziecko o danej nazwie lub null</returns>
        public static XElement GetElementByName(this XElement element, string name)
        {
            return element.Elements().Where(w => w.IsNamed(name)).FirstOrDefault();
        }

        /// <summary>
        /// Zwraca pierwszy atrybut elementu o określonej nazwie
        /// </summary>
        /// <param name="element">Element którego atybuty są sprawdzane</param>
        /// <param name="name">Nazwa wyszukiwanego atrybutu</param>
        /// <returns></returns>
        public static XAttribute GetAttributeByName(this XElement element, string name)
        {
            return element.Attributes().Where(w => w.Name.LocalName.ToLower() == name?.ToLower()).FirstOrDefault();
        }

        /// <summary>
        /// Sprawdza czy dany element ma daną nazwe
        /// </summary>
        /// <param name="element">Element</param>
        /// <param name="name">Sprawdzana nazwa</param>
        /// <returns></returns>
        public static bool IsNamed(this XElement element, string name)
        {
            return element.Name.LocalName.ToLower() == name.ToLower();
        }
    }
}
