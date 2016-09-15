using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SharpXMPP.XMPP.Client.Disco.Elements
{
    public class DiscoInfo : Stanza
    {
        public DiscoInfo()
            : base(XNamespace.Get(Namespaces.DiscoInfo) + "query")
        {
        }

        public Identity Identity
        {
            get { return Parse<Identity>(Element(XNamespace.Get(Namespaces.DiscoInfo) + "identity")); }
            set
            {
                SetElementValue(XNamespace.Get(Namespaces.DiscoInfo) + "identity", string.Empty);
                var id = Element(XNamespace.Get(Namespaces.DiscoInfo) + "identity");
                id.SetAttributeValue("category", value.Category);
                id.SetAttributeValue("type", value.IdentityType);
                id.SetAttributeValue("name", value.IdentityName);
            }
        }

        public string Node
        {
            get
            {
                var node = Attribute("node");
                return node == null ? string.Empty : node.Value;
            }
            set
            {
                SetAttributeValue("node", value);
            }
        }

        public List<string> Features
        {
            get
            {
                return
                    Elements(XNamespace.Get(Namespaces.DiscoInfo) + "feature").Select(el => el.Attribute("var").Value).
                        ToList();
            }
            set
            {
                value.ForEach((el) =>
                                  {
                                      var feature = new XElement(XNamespace.Get(Namespaces.DiscoInfo) + "feature");
                                      feature.SetAttributeValue("var", el);
                                      Add(feature);
                                  });
            }


        }
    }
}
