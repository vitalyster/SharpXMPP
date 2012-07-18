using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SharpXMPP.XMPP.Component.Elements
{
    public class Handshake : Stanza
    {
        public Handshake()
            : base(XNamespace.Get(Namespaces.JabberComponentAccept) + "handshake")
        {
        }
    }
}
