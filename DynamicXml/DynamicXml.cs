using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace DynamicXml
{
    public class DynamicXml :
        DynamicObject
    {
        private readonly XContainer _root;

        private DynamicXml(
            XContainer root)
        {
            _root = root;
        }

        public static DynamicXml Parse(
            string text)
        {
            return new DynamicXml(XDocument.Parse(text));
        }

        public static DynamicXml Load(
            TextReader reader)
        {
            return new DynamicXml(XDocument.Load(reader));
        }

        public static DynamicXml Load(
            Stream stream)
        {
            return new DynamicXml(XDocument.Load(stream));
        }

        public static DynamicXml Load(
            string filename)
        {
            return new DynamicXml(XDocument.Load(filename));
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            foreach (var name in _root.Elements().Select(s => s.Name.LocalName).Distinct())
            {
                yield return name;
            }

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
                        yield return "@text";
                    }
                }
            }
        }

        public override bool TryGetMember(
            GetMemberBinder binder,
            out object result)
        {
            result = null;

            if (_root is XElement e)
            {
                if (binder.Name == "@text")
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
                result = nodes.Select(n => n.HasElements ? (object)new DynamicXml(n) : n.Value).ToList();

                return true;
            }

            var node = nodes.FirstOrDefault();

            if (node != null)
            {
                result = node.HasElements || node.Attributes().Any(a => !a.IsNamespaceDeclaration) ? (object)new DynamicXml(node) : node.Value;

                return true;
            }

            return false;
        }
    }
}
