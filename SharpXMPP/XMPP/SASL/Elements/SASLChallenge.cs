using System.Xml.Linq;

namespace SharpXMPP.SASL.Elements
{
    public class SASLChallenge : XElement
    {
        public SASLChallenge()
            : base(XNamespace.Get(Namespaces.XmppSasl) + "challenge")
        {
        }
    }
}
