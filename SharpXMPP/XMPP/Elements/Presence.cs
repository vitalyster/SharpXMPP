using System.Xml.Linq;

namespace SharpXMPP.XMPP.Elements
{
    public class Presence : XElement
    {
        public Presence()
            : base(XNamespace.Get(Namespaces.JabberClient) + "presence")
        {
            
        }
    }
}
