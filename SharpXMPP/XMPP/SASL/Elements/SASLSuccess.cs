using System.Xml.Linq;

namespace SharpXMPP.SASL.Elements
{
    public class SASLSuccess : XElement
    {
        public SASLSuccess()
            : base(XNamespace.Get(Namespaces.XmppSasl) + "success")
        {
        }
    }
}
