using System.Xml.Linq;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using NUnit.Framework;

namespace DynamicXml.Test
{
    public class DynamicXmlTests
    {
        [TestCase("<a value=\"13\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">Testando</a>",
            ExpectedResult = "{\"a\":{\"value\":\"13\",\"_text\":\"Testando\"}}")]
        [TestCase("<a value=\"13\">Testando</a>",
            ExpectedResult = "{\"a\":{\"value\":\"13\",\"_text\":\"Testando\"}}")]
        [TestCase("<a><b>Texto 1</b><b>Texto 2</b></a>",
            ExpectedResult = "{\"a\":{\"b\":[\"Texto 1\",\"Texto 2\"]}}")]
        [TestCase("<a xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><xsi:b>Texto</xsi:b></a>",
            ExpectedResult = "{\"a\":{\"b\":\"Texto\"}}")]
        [TestCase("<a xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">Testando</a>",
            ExpectedResult = "{\"a\":\"Testando\"}")]
        [TestCase("<a><Testando name=\"123\"><b n=\"123\">Texto</b></Testando></a>",
            ExpectedResult = "{\"a\":{\"Testando\":{\"name\":\"123\",\"b\":{\"n\":\"123\",\"_text\":\"Texto\"}}}}")]
        [TestCase("<a name=\"Name\" />",
            ExpectedResult = "{\"a\":{\"name\":\"Name\"}}")]
        public string Test_conversion(
            string text)
        {
            var obj = DynamicXml.Parse(text);

            return JsonConvert.SerializeObject(obj);
        }

        [TestCase("<a name=\"Name\">Text</a>", ExpectedResult = "Text")]
        public string Test_get_text_property(
            string text)
        {
            var obj = DynamicXml.Parse(text);

            return obj.a._text;
        }

        [TestCase("<a name=\"Name\">Text</a>", ExpectedResult = "Text")]
        public string Test_change_text_property_name(
            string text)
        {
            var obj = DynamicXml.Parse(text, new DynamicXmlOptions { TextFieldName = "__text" });

            return obj.a.__text;
        }

        [TestCase("<a>Text</a>")]
        public void Test_get_property_thows_exception(
            string text)
        {
            var obj = DynamicXml.Parse(text);

            Assert.Catch(typeof(RuntimeBinderException), () => { var _ = obj.b; });
        }

        [TestCase("<a>Text</a>")]
        public void Test_get_property_returns_null_and_not_thows_exception(
            string text)
        {
            var obj = DynamicXml.Parse(text, new DynamicXmlOptions { ThrowOnGet = false });

            Assert.That(obj.b, Is.Null);
        }

        [TestCase("<a>Text</a>")]
        public void Test_create_from_xdocument(
            string text)
        {
            var xDocument = XDocument.Parse(text).ToDynamic();

            Assert.That(xDocument, Is.Not.Null);
        }

        [TestCase("<a>Text</a>")]
        public void Test_create_from_xelement(
            string text)
        {
            var xDocument = XDocument.Parse(text).Root.ToDynamic();

            Assert.That(xDocument, Is.Not.Null);
        }
    }
}