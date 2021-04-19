using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace DynamicXml
{
    public class DynamicXmlOptions
    {
        public static DynamicXmlOptions Default { get; set; } = new DynamicXmlOptions();

        public string TextFieldName { get; set; } = "_text";
        public bool ThrowOnGet { get; set; } = true;
    }

    public class DynamicXml :
        DynamicObject
    {
        private readonly XContainer _root;
        private readonly DynamicXmlOptions _options;

        public DynamicXml(
            XContainer root,
            DynamicXmlOptions options = null)
        {
            _root = root;
            _options = options ?? DynamicXmlOptions.Default;
        }

        public static dynamic Parse(
            string text,
            DynamicXmlOptions options = null)
        {
            return new DynamicXml(XDocument.Parse(text), options);
        }

        public static dynamic Load(
            TextReader reader,
            DynamicXmlOptions options = null)
        {
            return new DynamicXml(XDocument.Load(reader), options);
        }

        public static dynamic Load(
            Stream stream,
            DynamicXmlOptions options = null)
        {
            return new DynamicXml(XDocument.Load(stream), options);
        }

        public static dynamic Load(
            string filename,
            DynamicXmlOptions options)
        {
            return new DynamicXml(XDocument.Load(filename), options);
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            if (_root is XElement e)
            {
                var attributes = e.Attributes().Where(w => !w.IsNamespaceDeclaration);

                if (attributes.Any())
                {
                    foreach (var xAttribute in attributes)
                    {
                        if (xAttribute.IsNamespaceDeclaration) continue;

                        yield return xAttribute.Name.LocalName;
                    }

                    if (!e.IsEmpty && !e.HasElements)
                    {
                        yield return _options.TextFieldName;
                    }
                }
            }

            foreach (var name in _root.Elements().Select(s => s.Name.LocalName).Distinct())
            {
                yield return name;
            }
        }

        public IEnumerable AsEnumerable(
            string name)
        {
            var nodes = _root.Elements().Where(w => w.Name.LocalName == name);

            return nodes.Select(n => n.HasElements ? (object) new DynamicXml(n, _options) : n.Value);
        }

        public override bool TryGetMember(
            GetMemberBinder binder,
            out object result)
        {
            result = null;

            if (_root is XElement e)
            {
                if (binder.Name == _options.TextFieldName)
                {
                    result = e.Value;

                    return true;
                }

                var att = e.Attribute(binder.Name);

                if (att != null)
                {
                    result = att.Value;

                    return true;
                }
            }

            var nodes = _root.Elements().Where(w => w.Name.LocalName == binder.Name);

            if (nodes.Count() > 1)
            {
                result = nodes.Select(n => n.HasElements ? (object)new DynamicXml(n, _options) : n.Value);

                return true;
            }

            var node = nodes.FirstOrDefault();

            if (node != null)
            {
                result = node.HasElements || node.Attributes().Any(a => !a.IsNamespaceDeclaration) ? (object)new DynamicXml(node, _options) : node.Value;

                return true;
            }

            return !_options.ThrowOnGet;
        }
    }
}
