using System.Xml.Linq;

namespace SharpXMPP.Client.Elements
{
    public class Presence : XElement
    {
        public Presence()
            : base(XNamespace.Get(Namespaces.JabberClient) + "presence")
        {
            
        }
    }
}
