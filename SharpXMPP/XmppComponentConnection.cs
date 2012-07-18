using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using SharpXMPP.XMPP;
using SharpXMPP.XMPP.Component.Elements;

namespace SharpXMPP
{
    public class XmppComponentConnection : XmppTcpConnection
    {
        protected override int TcpPort
        {
            get { return 5347; }
            set { throw new NotImplementedException(); }
        }

        protected override IEnumerable<IPAddress> HostAddresses
        {
            get { return new Collection<IPAddress> { IPAddress.Loopback }; }
            set { throw new NotImplementedException(); }
        }

        public XmppComponentConnection(JID jid, string secret) : base(jid, secret)
        {
            StreamStart += (sender, id) => SendHandshake(id);
        }

        void SendHandshake(string streamID)
        {
            var handshake = new Handshake();
            var sha = System.Security.Cryptography.SHA1.Create();
            var shaHash = sha.ComputeHash(Encoding.UTF8.GetBytes(string.Concat(streamID, Password)));
            handshake.Value = BitConverter.ToString(shaHash).Replace("-", "").ToLower();
            Send(handshake);
        }

        protected override void OpenXmppStream()
        {
            Writer.WriteStartElement("stream", "stream", Namespaces.Streams);
            Writer.WriteAttributeString("xmlns", Namespaces.JabberComponentAccept);
            Writer.WriteAttributeString("version", "1.0");
            Writer.WriteAttributeString("to", Jid.Domain);
            Writer.WriteRaw("");
            Writer.Flush();
        }
    }
}
