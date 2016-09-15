using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;
using SharpXMPP.XMPP;
using SharpXMPP.XMPP.Client.Capabities;
using SharpXMPP.XMPP.Client.Disco.Elements;
using SharpXMPP.XMPP.Client.Elements;
using System;
using SharpXMPP.XMPP.Stream.Elements;

namespace SharpXMPP
{
    public class ConnFailedArgs
    {
        public string Message { get; set; }
    }

    public class ElementArgs
    {
        public bool IsInput { get; set; }
        public XElement Stanza { get; set; }
    }

    public class SignedInArgs
    {
        public JID Jid { get; set; }
    }   

    public abstract class XmppConnection
    {
        private readonly string _ns;

        public string Namespace
        {
            get
            {
                return _ns;
            }
        }
        protected XmppConnection(string ns)
        {
            queries = new Dictionary<string, Action<XMPPIq>>();
            _ns = ns;
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
        }

        public Features Features { get; set; }

        public JID Jid { get; set; }

        public delegate void ConnectionFailedHandler(XmppConnection sender, ConnFailedArgs e);

        public event ConnectionFailedHandler ConnectionFailed = delegate {};

        protected void OnConnectionFailed(ConnFailedArgs e)
        {
            ConnectionFailed(this, e);
        }

        public delegate void StreamStartHandler(XmppConnection sender, string streamId);

        public event StreamStartHandler StreamStart = delegate { };
 
        protected void OnStreamStart(string streamId)
        {
            StreamStart(this, streamId);
        }

        public delegate void SignedInHandler(XmppConnection sender, SignedInArgs e);

        public event SignedInHandler SignedIn = delegate {};

        protected void OnSignedIn(SignedInArgs e)
        {
            SignedIn(this, e);
        }

        public delegate void ElementHandler(XmppConnection sender, ElementArgs e);

        public event ElementHandler Element = delegate {};

        protected void OnElement(ElementArgs e)
        {
            Element(this, e);
        }

        public delegate void IqHandler(XmppConnection sender, XMPPIq e);

        protected event IqHandler Iq = delegate {};

        protected void OnIq(XMPPIq e)
        {
            if (e.IqType == XMPPIq.IqTypes.result 
                || e.IqType == XMPPIq.IqTypes.error
                && queries.ContainsKey(e.ID))
            {
                queries[e.ID](e);
                queries.Remove(e.ID);
            }
            else
            {
                // get, set
                Iq(this, e);
            }
            
        }

        public delegate void MessageHandler(XmppConnection sender, XMPPMessage e);

        public event MessageHandler Message = delegate { };

        protected void OnMessage(XMPPMessage e)
        {
            Message(this, e);
        }

        public delegate void PresenceHandler(XmppConnection sender, XMPPPresence e);

        public event PresenceHandler Presence = delegate { };

        protected void OnPresence(XMPPPresence e)
        {
            Presence(this, e);
        }

        private CapabilitiesManager _caps;
        public CapabilitiesManager Capabilities
        {
            get { return _caps ?? (_caps = new CapabilitiesManager()); }
            set { _caps = value; }
        }

        public abstract XElement NextElement();

        public virtual void Send(XElement data)
        {
            OnElement(new ElementArgs { Stanza = data, IsInput = false });
        }

        Dictionary<string, Action<XMPPIq>> queries;

        public void Query(XMPPIq request, Action<XMPPIq> response)
        {
            queries.Add(request.ID, response);
            Send(request);
        }

        public abstract void SessionLoop();
        public abstract void Connect();
    }
}
