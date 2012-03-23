using System.Xml.Linq;

namespace SharpXMPP.XMPP.Client.Elements
{
    public class Presence : XElement
    {
        public Presence()
            : base(XNamespace.Get(Namespaces.JabberClient) + "presence")
        {
            
        }
    }
}
