using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using SharpXMPP.XMPP;
using SharpXMPP.XMPP.Client.Capabities;
using SharpXMPP.XMPP.Client.Disco.Elements;

namespace SharpXMPP
{
    public class XmppClientConnection : XmppTcpConnection
    {
        public XmppClientConnection(JID jid, SecureString password)
            : base(jid, password)
        {
            Capabilities = new CapabilitiesManager
            {
                Identity = new Identity
                {
                    Category = "client",
                    IdentityType = "pc",
                    IdentityName = "SharpXMPP"
                },

                Node = "http://bggg.net.ru/caps",
                Features = new List<string>
                {
                    Namespaces.DiscoInfo,
                    Namespaces.DiscoItems
                }
            };
        }

        protected override int TcpPort
        {
            get { return 5222; }
            set { throw new NotImplementedException(); }
        }

        protected override IEnumerable<IPAddress> HostAddresses
        {
            get
            {
                var addresses = new List<IPAddress>();
                DNS.ResolveXMPPClient(Jid.Domain).ForEach(d => addresses.AddRange(Dns.GetHostAddresses(d.Host)));
                return addresses;
            }
            set { throw new NotImplementedException(); }
        }

        protected override void OpenXmppStream()
        {
            Writer.WriteStartElement("stream", "stream", Namespaces.Streams);
            Writer.WriteAttributeString("xmlns", Namespaces.JabberClient);
            Writer.WriteAttributeString("version", "1.0");
            Writer.WriteAttributeString("to", Jid.Domain);
            Writer.WriteRaw("");
            Writer.Flush();
        }
    }
}
