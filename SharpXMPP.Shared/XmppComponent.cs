using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Security;
using System.Text;
using SharpXMPP.XMPP;
using SharpXMPP.XMPP.Component.Elements;

namespace SharpXMPP
{
    public class XmppComponent : XmppTcpConnection
    {
        protected override int TcpPort
        {
            get { return 5347; }
            set { throw new NotImplementedException(); }
        }

        public XmppComponent(JID jid, string secret)
            : base(Namespaces.JabberComponentAccept, jid, secret)
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
    }
}
