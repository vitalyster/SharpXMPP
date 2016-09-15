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
                return Element(XNamespace.Get(Namespaces.JabberClient) + "body") == null ? string.Empty : Element(XNamespace.Get(Namespaces.JabberClient) + "body").Value;
            }
            set
            {
                var body = Element(XNamespace.Get(Namespaces.JabberClient) + "body") ??
                           new XElement(XNamespace.Get(Namespaces.JabberClient) + "body");
                body.SetValue(new XCData(value));
                Add(body);
            }
        }
    }
}
