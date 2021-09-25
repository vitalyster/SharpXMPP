using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using DnsClient;
using SharpXMPP.XMPP;
using SharpXMPP.XMPP.Bind.Elements;
using SharpXMPP.XMPP.Client.Capabities;
using SharpXMPP.XMPP.Client.Disco.Elements;
using SharpXMPP.XMPP.Client.Elements;
using SharpXMPP.XMPP.Framing.Elements;
using SharpXMPP.XMPP.SASL;
using SharpXMPP.XMPP.SASL.Elements;
using SharpXMPP.XMPP.Stream.Elements;
using WebSocket4Net;

namespace SharpXMPP
{
    public class XmppWebSocketConnection : XmppConnection
    {
        private WebSocket _connection;
        private string websocketUri;

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

        public XmppWebSocketConnection(JID jid, string password)
        : this (jid, password, string.Empty) { }

        public XmppWebSocketConnection(JID jid, string password, string websocketUri)
            : base("")
        {
            this.websocketUri = websocketUri;
            Jid = jid;
            Password = password;
            Capabilities = new CapabilitiesManager
            {
                Identity = new Identity
                {
                    Category = "client",
                    IdentityType = "mobile",
                    IdentityName = "SharpXMPP"
                },

                Node = "https://github.com/vitalyster/SharpXMPP",
                Features = new List<string>
                {
                    Namespaces.DiscoInfo,
                    Namespaces.DiscoItems
                }
            };
        }
        public override XElement NextElement()
        {
            throw new NotImplementedException();
        }

        public void ReadStreamStart(string data, string defaultNamespace = Namespaces.JabberClient)
        {
            var mngr = Stanza.Parse<Open>(XElement.Parse(data));
            OnStreamStart(mngr.ID);
        }

        public void RestartStream()
        {
            var open = new Open(Jid.Domain);
            _connection.Send(open.ToString());
            OnElement(new ElementArgs
            {
                IsInput = false,
                Stanza = open
            });
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


        public override Task ConnectAsync(CancellationToken token)
        {
            return Task.Run(async () =>
            {
                if (string.IsNullOrEmpty(websocketUri)) {
                    var lookup = new LookupClient();
                    var response = await lookup.QueryAsync(
                        "_xmppconnect." + Jid.Domain,
                        QueryType.TXT,
                        cancellationToken: token);
                    if (response.Answers.TxtRecords().Any())
                    {
                        foreach (var srv in response.Answers.TxtRecords())
                        {
                            foreach (var addr in srv.Text)
                            {
                                if (addr.StartsWith("_xmpp-client-websocket"))
                                {
                                    websocketUri = addr.Split('=')[1];
                                    break;
                                }
                            }
                        }
                    }
                }
                if (string.IsNullOrEmpty(websocketUri))
                {
                    OnConnectionFailed(new ConnFailedArgs
                    {
                        Message = "WebSocket URI is not resolved or set."
                    });
                    return;
                }
                _connection = new WebSocket(websocketUri, "xmpp", WebSocketVersion.Rfc6455);
                _connection.Opened += (sender, args) =>

                {
                    _currentState = XmppConnectionState.Connected;
                    RestartStream();
                };
                _connection.MessageReceived += (sender, args) =>
                {
                    OnElement(new ElementArgs
                    {
                        IsInput = true,
                        Stanza = XElement.Parse(args.Message)
                    });
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
                        var currentStanza = XElement.Parse(args.Message);
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
                                        Message = "supported sasl mechanism not available"
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
                                            Message = currentStanza.Value
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
                                var iq = new XMPPIq(XMPPIq.IqTypes.set);
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
                                        Message = "bind failed"
                                    });
                                    _currentState = XmppConnectionState.Disconnected;
                                }
                                else
                                {
                                    var sess =
                                        new XElement(
                                            XNamespace.Get(Namespaces.XmppSession) +
                                            "session");
                                    var sessIq = new XMPPIq(XMPPIq.IqTypes.set);
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
                                OnSignedIn(new SignedInArgs { Jid = Jid });
                                _currentState = XmppConnectionState.StreamNegotiated;
                                break;
                            case XmppConnectionState.StreamNegotiated:
                                if (currentStanza.Name.LocalName.Equals("iq"))
                                {
                                    OnIq(Stanza.Parse<XMPPIq>(currentStanza));
                                }
                                if (currentStanza.Name.LocalName.Equals("message"))
                                {
                                    OnMessage(Stanza.Parse<XMPPMessage>(currentStanza));
                                }
                                if (currentStanza.Name.LocalName.Equals("presence"))
                                {
                                    OnPresence(Stanza.Parse<XMPPPresence>(currentStanza));
                                }
                                break;
                            default:
                                throw new IOException("Invalid state");
                        }
                    }
                };
                _connection.Open();
            }, token);
        }
    }
}
