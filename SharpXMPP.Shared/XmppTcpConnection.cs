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
using DnsClient;
using SharpXMPP.Compat;
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
        private object _terminationLock = new object();
        private TcpClient _client;

        /// <summary>
        /// A list of name servers to use for the DNS lookup. Uses 1.1.1.1 by default. If empty, then will use the name
        /// servers configured by the local network adapter(s).
        /// </summary>
        public IPAddress[] NameServers { get; set; } = { IPAddress.Parse("1.1.1.1") };

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

        private void TerminateTcpConnection()
        {
            // There are two callers for this method: connection timeout and external dispose. This lock is placed in
            // case of a race condition between the two.
            lock (_terminationLock)
            {
                // NOTE: Client is explicitly Disposable on older runtimes, so cast is required.
                ((IDisposable)_client)?.Dispose();
                _client = null;
                Writer?.Dispose();
                Writer = null;
                Reader?.Dispose();
                Reader = null;
                ConnectionStream?.Dispose();
                ConnectionStream = null;
            }
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
                TerminateTcpConnection();
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
            List<IPAddress> HostAddresses = await ResolveHostAddresses(token);
            await ConnectOverTcp(HostAddresses, token);

            RestartXmlStreams();

            Features features = GetServerFeatures();
            var tlsSupported = await InitTlsIfSupported(features, token);
            if (tlsSupported)
            {
                features = GetServerFeatures();
            }

            await StartAuthentication(features, token);
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

        private async Task ConnectOverTcp(List<IPAddress> HostAddresses, CancellationToken cancellationToken)
        {
            TerminateTcpConnection();

            _client = new TcpClient();
            try
            {
                // TODO: check ports
                await _client.ConnectWithCancellationAsync(HostAddresses.ToArray(), TcpPort, cancellationToken);
                ConnectionStream = _client.GetStream();
            }
            catch
            {
                ((IDisposable)_client).Dispose();
                _client = null;
                throw;
            }
        }

        private Task StartAuthentication(Features features, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();

            void RunCatching(Action act)
            {
                try
                {
                    act();
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }

            Task.Run(() => RunCatching(() =>
            {
                var authenticator = SASLHandler.Create(features.SaslMechanisms, Jid, Password);
                if (authenticator == null)
                {
                    OnConnectionFailed(new ConnFailedArgs { Message = "supported sasl mechanism not available" });
                    tcs.TrySetResult(false);
                    return;
                }
                authenticator.Authenticated += _ => RunCatching(() =>
                {
                    RestartXmlStreams();
                    var session = new SessionHandler();
                    session.SessionStarted += connection => RunCatching(() =>
                    {
                        OnSignedIn(new SignedInArgs { Jid = connection.Jid });
                        tcs.TrySetResult(true);
                    });
                    // TODO make async
                    // Locks stream with SessionLoop
                    session.Start(this);
                });
                authenticator.AuthenticationFailed += _ => RunCatching(() =>
                {
                    OnConnectionFailed(new ConnFailedArgs { Message = "Authentication failed" });
                    tcs.TrySetResult(true);
                });

                using (cancellationToken.Register(TerminateTcpConnection))
                    authenticator.Start(this);

                tcs.TrySetResult(false);
            }), cancellationToken);
            return tcs.Task;
        }


        private async Task<List<IPAddress>> ResolveHostAddresses(CancellationToken cancellationToken)
        {
            List<IPAddress> HostAddresses = new List<IPAddress>();

            var lookup = NameServers.Length > 0 ? new LookupClient(NameServers) : new LookupClient();

            var response = await lookup.QueryAsync(
                "_xmpp-client._tcp." + Jid.Domain,
                QueryType.SRV,
                cancellationToken: cancellationToken);
            if (response.Answers.SrvRecords().Any())
            {
                foreach (var srv in response.Answers.SrvRecords())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var addresses = await Dns.GetHostAddressesAsync(srv.Target.Value);
                    HostAddresses.AddRange(addresses);
                }
            }
            else
            {
                cancellationToken.ThrowIfCancellationRequested();
                HostAddresses.AddRange(await Dns.GetHostAddressesAsync(Jid.Domain));
            }

            return HostAddresses;
        }

        private async Task<bool> InitTlsIfSupported(Features features, CancellationToken cancellationToken)
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
            await ((SslStream)ConnectionStream).AuthenticateAsClientWithCancellationAsync(
                Jid.Domain,
                cancellationToken);
            RestartXmlStreams();
            return true;
        }
    }
}
