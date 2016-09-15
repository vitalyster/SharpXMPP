using System.Xml.Linq;

namespace SharpXMPP.XMPP.TLS.Elements
{
    public class StartTLS : Stanza
    {
        public StartTLS() : base(XNamespace.Get(Namespaces.XmppTls) + "starttls")
        {
            
        }
    }
}
