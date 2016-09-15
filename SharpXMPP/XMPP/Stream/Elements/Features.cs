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
        
        public bool Tls
        {
            get
            {
                var tls = Element(XNamespace.Get(Namespaces.XmppTls) + "starttls");
                return (tls != null);
            }
        }
        public bool Bind
        {
            get
            {
                var bind = Element(XNamespace.Get(Namespaces.XmppBind) + "bind");
                return bind != null;
            }
        }
        public bool Session
        {
            get
            {
                var session = Element(XNamespace.Get(Namespaces.XmppSession) + "session");
                return session != null;
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
