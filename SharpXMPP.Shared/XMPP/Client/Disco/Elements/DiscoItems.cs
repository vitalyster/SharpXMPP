using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SharpXMPP.XMPP.Client.Disco.Elements
{
    public class DiscoItems : Stanza
    {
        public DiscoItems() : base(XNamespace.Get(Namespaces.DiscoItems) + "query")
        {
        }
    }
}
