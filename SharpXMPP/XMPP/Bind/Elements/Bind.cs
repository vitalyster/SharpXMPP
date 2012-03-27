using System.Xml.Linq;

namespace SharpXMPP.XMPP.Bind.Elements
{
    public class Bind : XElement
    {
        public Bind() : base(XNamespace.Get(Namespaces.XmppBind) + "bind")
        {
            
        }
        public Bind(string resource) : this()
        {
            Add(new XElement(XNamespace.Get(Namespaces.XmppBind) + "resource", resource));
        }
    }
}
