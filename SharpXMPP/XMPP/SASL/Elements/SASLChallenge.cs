using System.Xml.Linq;

namespace SharpXMPP.XMPP.SASL.Elements
{
    public class SASLChallenge : XElement
    {
        public SASLChallenge()
            : base(XNamespace.Get(Namespaces.XmppSasl) + "challenge")
        {
        }
    }
}
