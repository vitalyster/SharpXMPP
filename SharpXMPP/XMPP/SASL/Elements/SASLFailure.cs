using System.Xml.Linq;

namespace SharpXMPP.XMPP.SASL.Elements
{
    public class SASLFailure : XElement
    {
        public SASLFailure()
            : base(XNamespace.Get(Namespaces.XmppSasl) + "failure")
        {
        }
    }
}
