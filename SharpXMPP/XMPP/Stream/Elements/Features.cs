using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SharpXMPP.XMPP.Stream.Elements
{
    public class Features : Stanza
    {
        public Features() : base(XNamespace.Get(Namespaces.Streams) + "features")
        {
            
        }
        
        public bool TlsRequired
        {
            get
            {
                var tls = Element(XNamespace.Get(Namespaces.XmppTls) + "starttls");
                if (tls != null)
                {
                    return tls.Attribute("required") != null;
                }
                return false;
            }
        }
        public List<string> SaslMechanisms
        {
            get
            {
                var mechs = Element(XNamespace.Get(Namespaces.XmppSasl) + "mechanisms");
                return mechs != null ? mechs.Elements(XNamespace.Get(Namespaces.XmppSasl) + "mechanism").Select(e => e.Value).ToList() : new List<string>();
            }
        }
    }
}
