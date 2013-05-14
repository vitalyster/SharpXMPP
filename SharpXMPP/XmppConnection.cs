using System.Security;
using System.Xml.Linq;
using SharpXMPP.XMPP;
using SharpXMPP.XMPP.Client.Capabities;
using SharpXMPP.XMPP.Client.Elements;

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
        protected XmppConnection(JID jid, string password)
        {
            Jid = jid;
            Password = password;
        }
        public JID Jid { get; set; }

        protected readonly string Password;

        public delegate void ConnectionFailedHandler(object sender, ConnFailedArgs e);

        public event ConnectionFailedHandler ConnectionFailed = delegate {};

        protected void OnConnectionFailed(ConnFailedArgs e)
        {
            ConnectionFailed(this, e);
        }

        public delegate void StreamStartHandler(object sender, string streamID);

        public event StreamStartHandler StreamStart = delegate { };
 
        protected void OnStreamStart(string streamID)
        {
            StreamStart(this, streamID);
        }

        public delegate void SignedInHandler(object sender, SignedInArgs e);

        public event SignedInHandler SignedIn = delegate {};

        protected void OnSignedIn(SignedInArgs e)
        {
            SignedIn(this, e);
        }

        public delegate void ElementHandler(object sender, ElementArgs e);

        public event ElementHandler Element = delegate {};

        protected void OnElement(ElementArgs e)
        {
            Element(this, e);
        }

        public delegate void IqHandler(object sender, Iq e);

        public event IqHandler Iq = delegate {};

        protected void OnIq(Iq e)
        {
            Iq(this, e);
        }

        public delegate void MessageHandler(object sender, Message e);

        public event MessageHandler Message = delegate { };

        protected void OnMessage(Message e)
        {
            Message(this, e);
        }

        public CapabilitiesManager Capabilities { get; set; }
        
        public abstract XElement NextElement();

        public virtual void Send(XElement data)
        {
            OnElement(new ElementArgs { Stanza = data, IsInput = false });
        }

        public abstract void Connect();
    }
}
