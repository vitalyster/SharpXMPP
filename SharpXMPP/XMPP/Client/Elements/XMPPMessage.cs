using System;
using System.Xml.Linq;

namespace SharpXMPP.XMPP.Client.Elements
{
    public class XMPPMessage : Stanza
    {
        public XMPPMessage()
            : base(XNamespace.Get(Namespaces.JabberClient) + "message")
        {
            
        }

        public string Text
        {
            get
            {
                return Element(XNamespace.Get(Namespaces.JabberClient) + "body") == null ? null : Element(XNamespace.Get(Namespaces.JabberClient) + "body").Value;
            }
        }
    }
}
