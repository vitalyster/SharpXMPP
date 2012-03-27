using System.Xml.Linq;
using SharpXMPP.XMPP.Client.Capabities;
using SharpXMPP.XMPP.Client.Capabities.Elements;

namespace SharpXMPP.XMPP.Client.Elements
{
    public class Presence : Stanza
    {
        public Presence()
            : base(XNamespace.Get(Namespaces.JabberClient) + "presence")
        {
            
        }
        public Presence(CapabilitiesManager capabilities)
            : base(XNamespace.Get(Namespaces.JabberClient) + "presence")
        {
            Add(new Caps(capabilities.Node, capabilities.OurHash));
        }
    }
}
