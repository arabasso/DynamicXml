using System.Xml.Linq;

namespace DynamicXml
{
    public static class XNodeExtensions
    {
        public static dynamic ToDynamic(
            this XContainer xContainer)
        {
            return new DynamicXml(xContainer);
        }
    }
}