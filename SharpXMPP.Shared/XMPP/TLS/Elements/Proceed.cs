using System.Xml.Linq;

namespace SharpXMPP.XMPP.TLS.Elements
{
    public class Proceed : Stanza
    {
        public Proceed()
            : base(XNamespace.Get(Namespaces.XmppTls) + "proceed")
        {

        }
    }
}
