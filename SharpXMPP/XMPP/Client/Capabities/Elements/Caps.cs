using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SharpXMPP.XMPP.Client.Capabities.Elements
{
    public class Caps : Stanza
    {
        public Caps(string node, string hash)
            : base(XNamespace.Get(Namespaces.EntityCaps) + "c")
        {
            SetAttributeValue("hash", "sha-1");
            SetAttributeValue("node", node);
            SetAttributeValue("ver", hash);
        }
    }
}
