using System.Xml.Linq;

namespace SharpXMPP.XMPP.Client.Elements
{
    public class Message : XElement
    {
        public Message()
            : base(XNamespace.Get(Namespaces.JabberClient) + "message")
        {
            
        }

        public Message(XElement element) : this()
        {
            ReplaceAttributes(element.Attributes());
            ReplaceNodes(element.Nodes());
            Attribute("xmlns").Remove();
        }
    }
}
