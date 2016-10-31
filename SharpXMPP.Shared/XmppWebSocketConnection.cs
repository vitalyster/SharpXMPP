#if WEB_SOCKET
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Xml;
using System.Xml.Linq;
using SharpXMPP.XMPP;
using SharpXMPP.XMPP.Bind.Elements;
using SharpXMPP.XMPP.Client;
using SharpXMPP.XMPP.Client.Capabities;
using SharpXMPP.XMPP.Client.Disco;
using SharpXMPP.XMPP.Client.Disco.Elements;
using SharpXMPP.XMPP.Client.Elements;
using SharpXMPP.XMPP.SASL;
using SharpXMPP.XMPP.SASL.Elements;
using SharpXMPP.XMPP.Stream.Elements;
using WebSocket4Net;
using System.Text;
using SharpXMPP.XMPP.Client.Roster.Elements;

namespace SharpXMPP
{
    public class XmppWebSocketConnection : XmppConnection
    {
        private readonly WebSocket _connection;

        private enum XmppConnectionState
        {
            Disconnected,
            Connected,
            StreamInitiated,
            StreamAuthenticating,
            StreamAuthenticated,
            StreamResourceBindingRequest,
            StreamResourceBindingResponse,
            StreamSessionNoOp,
            StreamNegotiated
        };

        private XmppConnectionState _currentState = XmppConnectionState.Disconnected;

        private SASLHandler authenticator;

        public XmppWebSocketConnection(JID jid, string password, string websocketUri)
            : base(jid, password)
        {
            Capabilities = new CapabilitiesManager
            {
                Identity = new Identity
                {
                    Category = "client",
                    IdentityType = "mobile",
                    IdentityName = "SharpXMPP"
                },

                Node = "http://bggg.net.ru/caps",
                Features = new List<string>
                {
                    Namespaces.DiscoInfo,
                    Namespaces.DiscoItems
                }
            };
            IqTracker = new XMPP.Client.IqHandler(this)
            {
                ResponseHandlers = new Dictionary<string, ResponseHandler>(),
                PayloadHandlers = new List<PayloadHandler>
                          {
                              new InfoHandler(Capabilities),
                              new ItemsHandler()
                          }
            };
            Iq += (sender, iq) => IqTracker.Handle(iq);
// ReSharper disable RedundantArgumentDefaultValue
            _connection = new WebSocket(websocketUri, "xmpp", cookies: (List<KeyValuePair<string, string>>)null);
// ReSharper restore RedundantArgumentDefaultValue
            _connection.Opened += (sender, args) =>

                                      {
                                          _currentState = XmppConnectionState.Connected;
                                          RestartStream();
                                      };
            _connection.MessageReceived += (sender, args) =>
                                            {
                                                if (_currentState == XmppConnectionState.Connected)
                                                {
                                                    ReadStreamStart(args.Message);
                                                    _currentState = XmppConnectionState.StreamInitiated;
                                                } 
                                                else if (_currentState == XmppConnectionState.StreamAuthenticated)
                                                {
                                                    ReadStreamStart(args.Message);
                                                    _currentState =
                                                        XmppConnectionState.StreamResourceBindingRequest;
                                                            
                                                } 
                                                else
                                                {
                                                    var currentStanza = Stanza.Parse(args.Message);
                                                    OnElement(new ElementArgs
                                                    {
                                                        IsInput = false,
                                                        Stanza = currentStanza
                                                    });
                                                    var error = Stanza.Parse<StreamError>(currentStanza);
                                                    if (error != null)
                                                    {
                                                        OnConnectionFailed(new ConnFailedArgs { Message = error.Value });
                                                        return;
                                                    }
                                                    switch (_currentState)
                                                    {
                                                        case XmppConnectionState.StreamInitiated:

                                                            var features = Stanza.Parse<Features>(currentStanza);
                                                            
                                                            authenticator = SASLHandler.Create(features.SaslMechanisms,
                                                                                               Jid, Password);
                                                            if (authenticator == null)
                                                            {
                                                                OnConnectionFailed(new ConnFailedArgs
                                                                                       {
                                                                                           Message =
                                                                                               "supported sasl mechanism not available"
                                                                                       });
                                                                return;
                                                            }
                                                            var auth = new SASLAuth();
                                                            auth.SetAttributeValue("mechanism", authenticator.SASLMethod);
                                                            var authInit = authenticator.Initiate();
                                                            if (!string.IsNullOrEmpty(authInit))
                                                                auth.SetValue(authInit);
                                                            Send(auth);
                                                            _currentState = XmppConnectionState.StreamAuthenticating;
                                                            break;
                                                        case XmppConnectionState.StreamAuthenticating:
                                                            switch (currentStanza.Name.LocalName)
                                                            {
                                                                case "success":
                                                                    _currentState =
                                                                        XmppConnectionState.StreamAuthenticated;
                                                                    RestartStream();
                                                                    break;
                                                                case "failure":
                                                                    OnConnectionFailed(new ConnFailedArgs
                                                                                           {
                                                                                               Message =
                                                                                                   currentStanza.Value
                                                                                           });
                                                                    _currentState = XmppConnectionState.Disconnected;
                                                                    return;
                                                                case "challenge":
                                                                    var response = new SASLResponse();
                                                                    response.SetValue(
                                                                        authenticator.NextChallenge(currentStanza.Value));
                                                                    Send(response);
                                                                    break;
                                                            }
                                                            break;
                                                        case XmppConnectionState.StreamResourceBindingRequest:
                                                            // todo: parse features of negotiated stream
                                                            //Stanza.Parse<Features>(currentStanza);
                                                            var bind = new Bind(Jid.Resource);
                                                            var iq = new Iq(XMPP.Client.Elements.Iq.IqTypes.set);
                                                            iq.Add(bind);
                                                            Send(iq);
                                                            _currentState =
                                                                XmppConnectionState.StreamResourceBindingResponse;
                                                            break;
                                                        case XmppConnectionState.StreamResourceBindingResponse:
                                                            var bindedJid =
                                                                currentStanza.Element(
                                                                    XNamespace.Get(Namespaces.XmppBind) +
                                                                    "bind");
                                                            if (bindedJid == null)
                                                            {
                                                                OnConnectionFailed(new ConnFailedArgs
                                                                                       {
                                                                                           Message =
                                                                                               "bind failed"
                                                                                       });
                                                                _currentState = XmppConnectionState.Disconnected;
                                                            }
                                                            else
                                                            {
                                                                var sess =
                                                                    new XElement(
                                                                        XNamespace.Get(Namespaces.XmppSession) +
                                                                        "session");
                                                                var sessIq = new Iq(XMPP.Client.Elements.Iq.IqTypes.set);
                                                                sessIq.Add(sess);
                                                                Send(sessIq);
                                                                _currentState = XmppConnectionState.StreamSessionNoOp;
                                                                Jid =
                                                                    new JID(
                                                                        bindedJid.Element(
                                                                            XNamespace.Get(Namespaces.XmppBind) + "jid")
                                                                                 .Value);
                                                            }
                                                            break;
                                                        case XmppConnectionState.StreamSessionNoOp:
                                                            OnSignedIn(new SignedInArgs {Jid = Jid});
                                                            Roster.Query(this);
                                                            var initPresence = new Presence(Capabilities);
                                                            Send(initPresence);
                                                            _currentState = XmppConnectionState.StreamNegotiated;
                                                            break;
                                                        case XmppConnectionState.StreamNegotiated:
                                                            if (currentStanza.Name.LocalName.Equals("iq"))
                                                            {
                                                                OnIq(Stanza.Parse<Iq>(currentStanza));
                                                            }
                                                            if (currentStanza.Name.LocalName.Equals("message"))
                                                            {
                                                                OnMessage(Stanza.Parse<Message>(currentStanza));
                                                            }
                                                            break;
                                                        default:
                                                            throw new IOException("Invalid state");
                                                    }
                                                }
                                            };

        }
        public override XElement NextElement()
        {
            throw new NotImplementedException();
        }

        public void ReadStreamStart(string data, string defaultNamespace = Namespaces.JabberClient)
        {
            var mngr = new XmlNamespaceManager(new NameTable());
            mngr.AddNamespace("", defaultNamespace);
            mngr.AddNamespace("stream", Namespaces.Streams);
            var xrs = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Auto };
            var reader = XmlReader.Create(new StringReader(data), xrs, new XmlParserContext(null, mngr, null,
                                                            XmlSpace.None));
            reader.MoveToContent();
            if (reader.LocalName.Equals("stream") && reader.NamespaceURI.Equals(Namespaces.Streams))
            {
                OnStreamStart(reader.GetAttribute("id"));
            }
        }

        public void RestartStream()
        {
            var xws = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment, Encoding = Encoding.UTF8};
            var sw = new StringWriter();
            var writer = XmlWriter.Create(sw, xws);            
            writer.WriteStartElement("stream", "stream", Namespaces.Streams);
            writer.WriteAttributeString("xmlns", Namespaces.JabberClient);
            writer.WriteAttributeString("version", "1.0");
            writer.WriteAttributeString("to", Jid.Domain);
            writer.WriteRaw("");
            writer.Flush();
            _connection.Send(sw.ToString());
        }

        public string ElementToString(XElement element)
        {
            var sw = new StringWriter();
            var xws = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment, OmitXmlDeclaration = true, Encoding = Encoding.UTF8 };
            var writer = XmlWriter.Create(sw, xws);
            element.WriteTo(writer);
            writer.WriteRaw("");
            writer.Flush();
            return sw.ToString();
        }

        public override void Send(XElement data)
        {
            _connection.Send(ElementToString(data));
            base.Send(data);
            
        }

        public override void Connect()
        {
            _connection.Open();
        }
    }
}
#endif