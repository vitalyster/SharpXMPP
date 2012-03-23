using System.Xml.Linq;

namespace SharpXMPP.SASL.Elements
{
    public class SASLFailure : XElement
    {
        public SASLFailure()
            : base(XNamespace.Get(Namespaces.XmppSasl) + "failure")
        {
        }
    }
}
