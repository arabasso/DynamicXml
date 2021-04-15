using System.Xml.Linq;

namespace DynamicXml
{
    public static class XDocumentoExtensions
    {
        public static dynamic ToDynamic(
            this XContainer xContainer)
        {
            return new DynamicXml(xContainer);
        }
    }
}