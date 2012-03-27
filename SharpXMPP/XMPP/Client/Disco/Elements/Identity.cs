using System.Collections.Generic;
using System.Xml.Linq;

namespace SharpXMPP.XMPP.Client.Disco.Elements
{
    public class Identity : XElement
    {
        public Identity() : base(XNamespace.Get(Namespaces.DiscoInfo) + "identity")
        {
            
        }

        public string Category { get; set; }
        public string IdentityName { get; set; }
        public string IdentityType { get; set; }

        public List<string> Features { get; set; }
    }
}
