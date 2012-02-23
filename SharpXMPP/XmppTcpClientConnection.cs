using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
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
            // TODO: SRV resolving
            ConnectionJID = jid;
            _password = password;
            _client = new TcpClient(ConnectionJID.Domain, 5222);
            ConnectionStream = _client.GetStream();
        }

        public System.IO.Stream ConnectionStream;

        protected XmlReader Reader;

        protected void RestartReader()
        {
            var init = "<stream:stream xmlns=\"jabber:client\" xmlns:stream=\"http://etherx.jabber.org/streams\" to=\""+ ConnectionJID.Domain + "\" version=\"1.0\">";
            ConnectionStream.Write(Encoding.UTF8.GetBytes(init), 0, init.Length);
            ConnectionStream.Flush();
            var xrs = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };
            Reader = XmlReader.Create(ConnectionStream, xrs);

        }

        public override XElement NextElement()
        {
            Reader.MoveToContent();
            Reader.Read();
            var result = XElement.Load(Reader.ReadSubtree());
            OnElement(new ElementArgs { Stanza = result, IsInput = true });
            return result;
        }

        public override void Send(XElement data)
        {
            OnElement(new ElementArgs { Stanza = data, IsInput = false });
            var bytes = Encoding.UTF8.GetBytes(data.ToString());
            ConnectionStream.Write(bytes, 0, bytes.Length);
            ConnectionStream.Flush();
        }

        public override void MainLoop()
        {
            RestartReader();
            var features = Deserealize<Features>(NextElement());
            if (features != null)
            {

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
                    var iq = new Iq(Iq.IqType.Set);
                    iq.Add(bind);
                    Send(iq);
                    var el4 = NextElement();
                    var jid = el4.Element("{urn:ietf:params:xml:ns:xmpp-bind}bind").Element("{urn:ietf:params:xml:ns:xmpp-bind}jid");
                    var sess = new XElement("{urn:ietf:params:xml:ns:xmpp-session}session");
                    var sessIq = new Iq(Iq.IqType.Set);
                    sessIq.Add(sess);
                    Send(sessIq);
                    var el5 = NextElement();
                    ConnectionJID = new JID(jid.ToString());
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
}
