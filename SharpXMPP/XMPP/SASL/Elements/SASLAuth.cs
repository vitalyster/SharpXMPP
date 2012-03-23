using System.Xml.Linq;

namespace SharpXMPP.XMPP.SASL.Elements
{
    public class SASLAuth : XElement
    {
        public SASLAuth() : base(XNamespace.Get(Namespaces.XmppSasl) + "auth")
        {
        }
    }
}
