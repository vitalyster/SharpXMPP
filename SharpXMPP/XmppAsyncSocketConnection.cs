using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using SharpXMPP.XMPP;
using SharpXMPP.XMPP.Bind.Elements;
using SharpXMPP.XMPP.Client;
using SharpXMPP.XMPP.Client.Disco;
using SharpXMPP.XMPP.Client.Disco.Elements;
using SharpXMPP.XMPP.Client.Elements;
using SharpXMPP.XMPP.SASL;
using SharpXMPP.XMPP.SASL.Elements;
using SharpXMPP.XMPP.Stream.Elements;
using SuperSocket.ClientEngine;

namespace SharpXMPP
{
    public class XmppAsyncSocketConnection : XmppConnection
    {
        private readonly AsyncTcpSession _client;
        private readonly PipeStream _connectionStream;

        protected XmlReader Reader;
        

        private readonly SecureString _password;

        public bool InitialPresence { get; set; }

        public XmppAsyncSocketConnection(JID jid, SecureString password)
        {
            Jid = jid;

            _password = password;

            _connectionStream = new PipeStream { BlockLastReadBuffer = false };
            //TODO: DNS SRV
            _client = new AsyncTcpSession(new DnsEndPoint(jid.Domain, 5222));
            Iq += (sender, iq) => new XMPP.Client.IqHandler(this)
            {
                PayloadHandlers = new List<PayloadHandler>
                          {
                              new InfoHandler(Capabilities),
                              new ItemsHandler()
                          }
            }.Handle(iq);
            _client.DataReceived += (sender, args) =>
                                        {
                                            _connectionStream.Write(args.Data, 0, args.Length);
                                        };
            _client.Connected += (sender, args) => Connect();
            _client.Connect();
        }

        protected void RestartXmlStreams()
        {
            /*var xws = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment, OmitXmlDeclaration = true };
            Writer = XmlWriter.Create(ConnectionStream, xws);
            Writer.WriteStartElement("stream", "stream", Namespaces.Streams);
            Writer.WriteAttributeString("xmlns", Namespaces.JabberClient);
            Writer.WriteAttributeString("version", "1.0");
            Writer.WriteAttributeString("to", JID.Domain);
            Writer.WriteRaw("");
            Writer.Flush();*/
            var data = string.Format("<stream:stream xmlns:stream=\"{0}\" xmlns=\"{1}\" version=\"1.0\" to=\"{2}\">", 
                Namespaces.Streams, Namespaces.JabberClient, Jid.Domain);
            _client.Send(Encoding.UTF8.GetBytes(data), 0, data.Length);
            var xrs = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment, IgnoreWhitespace = true};
            //Reader = XmlReader.Create(_connectionStream, xrs);
            
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
            base.Send(data);
            _client.Send(Encoding.UTF8.GetBytes(data.ToString()), 0, data.ToString().Length);
        }

        public void Close()
        {
            _client.Send(Encoding.UTF8.GetBytes("</stream:stream>"), 0, "</stream:stream>".Length);
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
                        OnIq(Stanza.Parse<Iq>(el));
                    }
                    if (el.Name.LocalName.Equals("message"))
                    {
                        OnMessage(Stanza.Parse<Message>(el));
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
            var features = Stanza.Parse<Features>(NextElement());
            
            var authenticator = SASLHandler.Create(features.SaslMechanisms, Jid, _password);
            if (authenticator == null)
            {
                OnConnectionFailed(new ConnFailedArgs { Message = "supported sasl mechanism not available" });
                return;
            }
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
            {
                OnConnectionFailed(new ConnFailedArgs { Message = "bind failed" });
                return;
            }
            var sess = new XElement(XNamespace.Get(Namespaces.XmppSession) + "session");
            var sessIq = new Iq(XMPP.Client.Elements.Iq.IqTypes.set);
            sessIq.Add(sess);
            Send(sessIq);
            NextElement(); // skip session result
            Jid = new JID(jid.Element(XNamespace.Get(Namespaces.XmppBind) + "jid").Value);
            OnSignedIn(new SignedInArgs { Jid = Jid });
            if (InitialPresence)
                Send(new Presence(Capabilities));
            SessionLoop();
        }
    }
}
