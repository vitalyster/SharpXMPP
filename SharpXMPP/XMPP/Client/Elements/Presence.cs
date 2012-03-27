using System.Xml.Linq;

namespace SharpXMPP.XMPP.Client.Elements
{
    public class Presence : Stanza
    {
        public Presence()
            : base(XNamespace.Get(Namespaces.JabberClient) + "presence")
        {
            
        }
    }
}
