using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SharpXMPP.XMPP.Client.Disco.Elements
{
    public class Identity : Stanza
    {
        public Identity()
            : base(XNamespace.Get(Namespaces.DiscoInfo) + "identity")
        {

        }

        public string Category
        {
            get { return Attribute("category").Value; }
            set
            {
                SetAttributeValue("category", value);
            }
        }

        public string IdentityName
        {
            get { return Attribute("name").Value; }
            set { SetAttributeValue("name", value); }
        }

        public string IdentityType
        {
            get { return Attribute("type").Value; }
            set { SetAttributeValue("type", value); }
        }

    }
}
