using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using SharpXMPP.Client;
using SharpXMPP.Stream;

namespace SharpXMPP
{
    public class XmppTcpClientConnection : XmppConnection
    {

        private readonly TcpClient _client;

        private readonly string _password;

        public bool InitialPresence { get; set; }

        public XmppTcpClientConnection(JID jid, string password)
        {
            ConnectionJID = jid;
            _password = password;
            var addresses = new List<IPAddress>();
            DnsResolver.ResolveXMPPClient(ConnectionJID.Domain).ForEach(d => addresses.AddRange(Dns.GetHostAddresses(d.Host)));
            _client = new TcpClient();
            _client.Connect(addresses.ToArray(), 5222); // TODO: check ports
            ConnectionStream = _client.GetStream();
        }

        public System.IO.Stream ConnectionStream;

        protected XmlReader Reader;

        protected void RestartReader()
        {
            var init = "<stream:stream xmlns=\"jabber:client\" xmlns:stream=\"http://etherx.jabber.org/streams\" to=\""+ ConnectionJID.Domain + "\" version=\"1.0\">";
            ConnectionStream.Write(Encoding.UTF8.GetBytes(init), 0, init.Length);
            var xrs = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };
            Reader = XmlReader.Create(ConnectionStream, xrs);

        }

        public override XElement NextElement()
        {
            Reader.MoveToContent();
            do
            {
                Reader.Read();
            } while (Reader.NodeType != XmlNodeType.Element);
            var result = XElement.Load(Reader.ReadSubtree());
            OnElement(new ElementArgs { Stanza = result, IsInput = true });
            return result;
        }

        public override void Send(XElement data)
        {
            OnElement(new ElementArgs { Stanza = data, IsInput = false });
            var bytes = Encoding.UTF8.GetBytes(data.ToString());
            ConnectionStream.Write(bytes, 0, bytes.Length);
        }

        public void Close()
        {
            const string data = "</stream:stream>";
            ConnectionStream.Write(Encoding.UTF8.GetBytes(data), 0, data.Length);
        }

        public override void MainLoop()
        {
            RestartReader();
            var features = Deserealize<Features>(NextElement());
            if (features == null) return;
            Send(new XElement("{urn:ietf:params:xml:ns:xmpp-tls}starttls"));
            var res = NextElement();
            if (res.Name.LocalName == "proceed")
            {
                ConnectionStream = new SslStream(ConnectionStream, true);
                ((SslStream)ConnectionStream).AuthenticateAsClient(ConnectionJID.Domain);
                RestartReader();
                NextElement();
            }

            var auth = new XElement("{urn:ietf:params:xml:ns:xmpp-sasl}auth");
            auth.SetAttributeValue("mechanism", "PLAIN");
            auth.SetValue(
                Convert.ToBase64String(Encoding.UTF8.GetBytes(ConnectionJID.BareJid + '\0' + ConnectionJID.User + '\0' + _password)));
            Send(auth);
            var el2 = NextElement();
            if (el2.Name.LocalName == "success")
            {
                RestartReader();
                var el3 = NextElement();
                var bind = new XElement("{urn:ietf:params:xml:ns:xmpp-bind}bind");
                var resource = new XElement("{urn:ietf:params:xml:ns:xmpp-bind}resource")
                                   {Value = ConnectionJID.Resource};
                bind.Add(resource);
                var iq = new Iq(Iq.IqTypes.Set);
                iq.Add(bind);
                Send(iq);
                var el4 = NextElement();
                var jid = el4.Element("{urn:ietf:params:xml:ns:xmpp-bind}bind").Element("{urn:ietf:params:xml:ns:xmpp-bind}jid");
                var sess = new XElement("{urn:ietf:params:xml:ns:xmpp-session}session");
                var sessIq = new Iq(Iq.IqTypes.Set);
                sessIq.Add(sess);
                Send(sessIq);
                var el5 = NextElement();
                ConnectionJID = new JID(jid.Value);
                OnSignedIn(new SignedInArgs {ConnectionJID = ConnectionJID});
                if (InitialPresence)
                    Send(new Presence());
                var task = Task.Factory.StartNew(() =>
                                                     {
                                                         while (true)
                                                         {
                                                             try
                                                             {
                                                                 NextElement();
                                                             }
                                                             catch (Exception e)
                                                             {
                                                                 OnConnectionFailed(new ConnFailedArgs { Message = e.Message });
                                                                 break;
                                                             }
                                                         }
                                                     });
                task.Wait();
            }
            else
            {
                OnConnectionFailed(new ConnFailedArgs { Message = "not-authorized" });
            }
        }

    }
}
