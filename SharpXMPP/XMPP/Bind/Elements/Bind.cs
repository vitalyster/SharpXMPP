using System;
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
            if (!String.IsNullOrEmpty(resource))
            {
                Add(new XElement(XNamespace.Get(Namespaces.XmppBind) + "resource", resource));
            }
        }
    }
}
