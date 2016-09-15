using System.Xml.Linq;
using SharpXMPP.XMPP.Client.Capabities;
using SharpXMPP.XMPP.Client.Capabities.Elements;

namespace SharpXMPP.XMPP.Client.Elements
{
    public class XMPPPresence : Stanza
    {
        public XMPPPresence()
            : base(XNamespace.Get(Namespaces.JabberClient) + "presence")
        {
            
        }
        public XMPPPresence(CapabilitiesManager capabilities)
            : base(XNamespace.Get(Namespaces.JabberClient) + "presence")
        {
            Add(new Caps(capabilities.Node, capabilities.OurHash));
        }
    }
}
