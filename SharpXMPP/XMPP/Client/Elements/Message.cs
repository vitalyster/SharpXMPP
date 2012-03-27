using System.Xml.Linq;

namespace SharpXMPP.XMPP.Client.Elements
{
    public class Message : Stanza
    {
        public Message()
            : base(XNamespace.Get(Namespaces.JabberClient) + "message")
        {
            
        }
    }
}
