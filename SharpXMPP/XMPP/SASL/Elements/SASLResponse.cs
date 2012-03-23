using System.Xml.Linq;

namespace SharpXMPP.XMPP.SASL.Elements
{
    public class SASLResponse : XElement
    {
        public SASLResponse()
            : base(XNamespace.Get(Namespaces.XmppSasl) + "response")
        {
        }
    }
}
