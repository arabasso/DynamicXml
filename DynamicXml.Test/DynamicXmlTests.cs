using Newtonsoft.Json;
using NUnit.Framework;

namespace DynamicXml.Test
{
    public class DynamicXmlTests
    {
        [TestCase("<a value=\"13\">Testando</a>", ExpectedResult = "{\"a\":{\"value\":\"13\",\"@text\":\"Testando\"}}")]
        [TestCase("<a><b>Texto 1</b><b>Texto 2</b></a>", ExpectedResult = "{\"a\":{\"b\":[\"Texto 1\",\"Texto 2\"]}}")]
        public string Test_conversion(
            string text)
        {
            dynamic obj = DynamicXml.Parse(text);

            return JsonConvert.SerializeObject(obj);
        }
    }
}