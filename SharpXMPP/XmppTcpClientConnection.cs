using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Xml;
using System.Xml.Linq;
using SharpXMPP.XMPP;
using SharpXMPP.XMPP.Bind.Elements;
using SharpXMPP.XMPP.Client.Elements;
using SharpXMPP.XMPP.SASL;
using SharpXMPP.XMPP.SASL.Elements;
using SharpXMPP.XMPP.Stream.Elements;
using SharpXMPP.XMPP.TLS.Elements;

namespace SharpXMPP
{
    public class XmppTcpClientConnection : XmppConnection
    {

        private readonly TcpClient _client;

        private readonly string _password;

        public bool InitialPresence { get; set; }

        public XmppTcpClientConnection(JID jid, string password)
        {
            Jid = jid;
            _password = password;
            var addresses = new List<IPAddress>();
            DNS.ResolveXMPPClient(Jid.Domain).ForEach(d => addresses.AddRange(Dns.GetHostAddresses(d.Host)));
            _client = new TcpClient();
            _client.Connect(addresses.ToArray(), 5222); // TODO: check ports
            ConnectionStream = _client.GetStream();
            Iq += (sender, iq) => new XMPP.Client.IqHandler(this).Handle(iq);
        }

        public System.IO.Stream ConnectionStream;

        protected XmlReader Reader;
        protected XmlWriter Writer;

        protected void RestartXmlStreams()
        {
            var xws = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment, OmitXmlDeclaration = true };
            Writer = XmlWriter.Create(ConnectionStream, xws);
            Writer.WriteStartElement("stream", "stream", Namespaces.Streams);
            Writer.WriteAttributeString("xmlns", Namespaces.JabberClient);
            Writer.WriteAttributeString("version", "1.0");
            Writer.WriteAttributeString("to", Jid.Domain);
            Writer.WriteRaw("");
            Writer.Flush();
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
            data.WriteTo(Writer);
            Writer.WriteRaw("");
            Writer.Flush();
        }

        public void Close()
        {
            Writer.WriteEndElement();
        }

        protected void SessionLoop()
        {
            while (true)
            {
                try
                {
                    var el = NextElement();
                    if (el.Name.LocalName.Equals("iq"))
                    {
                        OnIq(Stanza.Clone<Iq>(el));
                    }
                    if (el.Name.LocalName.Equals("message"))
                    {
                        OnMessage(Stanza.Clone<Message>(el));
                    }

                }
                catch (Exception e)
                {
                    OnConnectionFailed(new ConnFailedArgs { Message = e.Message });
                    break;
                }
            }
        }

        public override void Connect()
        {
            RestartXmlStreams();
            var features = Stanza.Clone<Features>(NextElement());
            if (features.TlsRequired || true)
            {
                Send(new StartTLS());
                var res = Stanza.Clone<Proceed>(NextElement());
                if (res != null)
                {
                    ConnectionStream = new SslStream(ConnectionStream, true);
                    ((SslStream)ConnectionStream).AuthenticateAsClient(Jid.Domain);
                    RestartXmlStreams();
                    NextElement();
                }
            }

            var authenticator = SASLHandler.Create(features.SaslMechanisms, Jid, _password);
            if (authenticator == null)
                OnConnectionFailed(new ConnFailedArgs { Message = "supported sasl mechanism not available" });
            var auth = new SASLAuth();
            auth.SetAttributeValue("mechanism", authenticator.SASLMethod);
            auth.SetValue(authenticator.Initiate());
            Send(auth);
            var authResponse = NextElement();
            while (authResponse.Name.LocalName != "success")
            {
                authenticator.NextChallenge(authResponse.Value);
                authResponse = NextElement();
            }
            RestartXmlStreams();
            NextElement(); // skip features
            var bind = new Bind(Jid.Resource);
            var iq = new Iq(XMPP.Client.Elements.Iq.IqTypes.set);
            iq.Add(bind);
            Send(iq);
            var el4 = NextElement();
            var jid = el4.Element(XNamespace.Get(Namespaces.XmppBind) + "bind");
            if (jid == null)
                OnConnectionFailed(new ConnFailedArgs { Message = "bind failed" });
            var sess = new XElement(XNamespace.Get(Namespaces.XmppSession) + "session");
            var sessIq = new Iq(XMPP.Client.Elements.Iq.IqTypes.set);
            sessIq.Add(sess);
            Send(sessIq);
            NextElement(); // skip session result
            Jid = new JID(jid.Element(XNamespace.Get(Namespaces.XmppBind) + "jid").Value);
            OnSignedIn(new SignedInArgs { Jid = Jid });
            if (InitialPresence)
                Send(new Presence());
            SessionLoop();
        }

    }
}
