using System.Xml.Linq;

namespace SharpXMPP.Client
{
    public class Message : XElement
    {
        public Message()
            : base(XNamespace.Get(Namespaces.JabberClient) + "message")
        {
            
        }

        public static Message CreateFrom(XElement element)
        {
            var result = new Message();
            result.ReplaceAttributes(element.Attributes());
            result.ReplaceNodes(element.Nodes());
            result.Attribute("xmlns").Remove();
            return result;
        }
    }
}
