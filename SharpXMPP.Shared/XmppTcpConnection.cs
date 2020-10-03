using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using SharpXMPP.Errors;
using SharpXMPP.XMPP;
using SharpXMPP.XMPP.Bind;
using SharpXMPP.XMPP.Client.Elements;
using SharpXMPP.XMPP.SASL;
using SharpXMPP.XMPP.Stream.Elements;
using SharpXMPP.XMPP.TLS.Elements;

namespace SharpXMPP
{
    public class XmppTcpConnection : XmppConnection
    {

        private TcpClient _client;

        protected virtual int TcpPort
        {
            get { return 5222; }
            set { throw new NotImplementedException(); }
        }


        public XmppTcpConnection(string ns, JID jid, string password) : base(ns)
        {
            Jid = jid;
            Password = password;
        }

        public System.IO.Stream ConnectionStream { get; private protected set; }

        protected XmlReader Reader;
        protected XmlWriter Writer;
        private bool _disposed;

        protected void RestartXmlStreams()
        {
            var xws = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment, OmitXmlDeclaration = true };
            Writer = XmlWriter.Create(ConnectionStream, xws);
            OpenXmppStream();
            var xrs = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };
            Reader = XmlReader.Create(ConnectionStream, xrs);

        }

        protected void OpenXmppStream()
        {
            Writer.WriteStartElement("stream", "stream", Namespaces.Streams);
            Writer.WriteAttributeString("xmlns", Namespace);
            Writer.WriteAttributeString("version", "1.0");
            Writer.WriteAttributeString("to", Jid.Domain);
            Writer.WriteRaw("");
            Writer.Flush();
        }

        public override XElement NextElement()
        {
            Reader.MoveToContent();
            if (Reader.LocalName.Equals("stream") && Reader.NamespaceURI.Equals(Namespaces.Streams))
            {
                OnStreamStart(Reader.GetAttribute("id"));
            }
            do
            {
                if (!Reader.Read())
                {
                    throw new XmppConnectionTerminatedException();
                }
            } while (Reader.NodeType != XmlNodeType.Element);
            var result = XElement.Load(Reader.ReadSubtree());
            OnElement(new ElementArgs { Stanza = result, IsInput = true });
            return result;
        }

        public override void Send(XElement data)
        {
            base.Send(data);
            data.WriteTo(Writer);
            Writer.WriteRaw("");
            Writer.Flush();
        }

        // In what context this method should be used?
        public void Close()
        {
            Writer.WriteEndElement();
        }

        public override void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                // NOTE: used this statement because faced issue with compilation under net451
                ((IDisposable)_client)?.Dispose();
                _client = null;
                Writer?.Dispose();
                Writer = null;
                Reader?.Dispose();
                Reader = null;
                ConnectionStream?.Dispose();
                ConnectionStream = null;
            }
            base.Dispose();
        }

        public void SessionLoop()
        {
            while (true)
            {
                try
                {
                    var el = NextElement();
                    if (el.Name.LocalName.Equals("iq"))
                    {
                        OnIq(Stanza.Parse<XMPPIq>(el));
                    }
                    if (el.Name.LocalName.Equals("message"))
                    {
                        OnMessage(Stanza.Parse<XMPPMessage>(el));
                    }
                    if (el.Name.LocalName.Equals("presence"))
                    {
                        OnPresence(Stanza.Parse<XMPPPresence>(el));
                    }
                }
                catch (Exception e)
                {
                    OnConnectionFailed(new ConnFailedArgs { Exception = e, Message = e.Message });
                    break;
                }
            }
        }

        public override async Task ConnectAsync(CancellationToken token)
        {
            List<IPAddress> HostAddresses = await ResolveHostAddresses();
            await ConnectOverTcp(HostAddresses);

            RestartXmlStreams();

            Features features = GetServerFeatures();
            var tlsSupported = await InitTlsIfSupported(features);
            if (tlsSupported)
            {
                features = GetServerFeatures();
            }

            await StartAuthentication(features);
        }

        public Task SessionLoopAsync() => SessionLoopAsync(CancellationToken.None);

        public Task SessionLoopAsync(CancellationToken token)
        {
            return Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var el = NextElement();
                        token.ThrowIfCancellationRequested();
                        if (el.Name.LocalName.Equals("iq"))
                        {
                            token.ThrowIfCancellationRequested();
                            OnIq(Stanza.Parse<XMPPIq>(el));
                        }
                        if (el.Name.LocalName.Equals("message"))
                        {
                            token.ThrowIfCancellationRequested();
                            OnMessage(Stanza.Parse<XMPPMessage>(el));
                        }
                        if (el.Name.LocalName.Equals("presence"))
                        {
                            token.ThrowIfCancellationRequested();
                            OnPresence(Stanza.Parse<XMPPPresence>(el));
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception e)
                    {
                        OnConnectionFailed(new ConnFailedArgs { Exception = e, Message = e.Message });
                        break;
                    }
                }
            }, token);
        }

        private Features GetServerFeatures() => Stanza.Parse<Features>(NextElement());

        private async Task ConnectOverTcp(List<IPAddress> HostAddresses)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(HostAddresses.ToArray(), TcpPort); // TODO: check ports
            ConnectionStream = _client.GetStream();
        }

        private Task StartAuthentication(Features features)
        {
            var tcs = new TaskCompletionSource<bool>();
            Task.Run(() =>
            {
                var authenticator = SASLHandler.Create(features.SaslMechanisms, Jid, Password);
                if (authenticator == null)
                {
                    OnConnectionFailed(new ConnFailedArgs { Message = "supported sasl mechanism not available" });
                    tcs.SetResult(false);
                    return;
                }
                authenticator.Authenticated += sender =>
                {
                    RestartXmlStreams();
                    var session = new SessionHandler();
                    session.SessionStarted += connection =>
                    {
                        OnSignedIn(new SignedInArgs { Jid = connection.Jid });
                        tcs.SetResult(true);
                    };
                    // TODO make async
                    // Locks stream with SessionLoop
                    session.Start(this);
                };
                authenticator.AuthenticationFailed += sender =>
                {
                    OnConnectionFailed(new ConnFailedArgs { Message = "Authentication failed" });
                    tcs.SetResult(true);
                };
                authenticator.Start(this);
            });
            return tcs.Task;
        }


        private async Task<List<IPAddress>> ResolveHostAddresses()
        {
            List<IPAddress> HostAddresses = new List<IPAddress>();

            var srvs = await Resolver.ResolveXMPPClient(Jid.Domain);
            if (srvs.Any())
            {
                foreach (var srv in srvs)
                {
                    var addresses = await Dns.GetHostAddressesAsync(srv.Host);
                    HostAddresses.AddRange(addresses);
                }
            }
            else
            {
                HostAddresses.AddRange(await Dns.GetHostAddressesAsync(Jid.Domain));
            }

            return HostAddresses;
        }

        private async Task<bool> InitTlsIfSupported(Features features)
        {
            if (!features.Tls)
            {
                return false;
            }

            Send(new StartTLS());
            var res = Stanza.Parse<Proceed>(NextElement());
            if (res == null)
            {
                return false;
            }

            ConnectionStream = new SslStream(ConnectionStream, true);
            await ((SslStream)ConnectionStream).AuthenticateAsClientAsync(Jid.Domain);
            RestartXmlStreams();
            return true;
        }
    }
}
