using System.Xml.Linq;

namespace SharpXMPP.Client
{
    public class Presence : XElement
    {
        public Presence()
            : base(XNamespace.Get(Namespaces.JabberClient) + "presence")
        {
            
        }
    }
}
