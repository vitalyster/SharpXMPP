using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using SharpXMPP.XMPP.Client.Elements;
using SharpXMPP.XMPP.Stream.Elements;

namespace SharpXMPP.XMPP.Bind
{
    public class SessionHandler
    {
        public delegate void SessionStartedHandler(XmppConnection sender);

        public event SessionStartedHandler SessionStarted = delegate { };

        protected virtual void OnSessionStarted(XmppConnection sender)
        {
            SessionStarted(sender);
        }
        public void Start(XmppTcpConnection connection)
        {
            connection.Features = Stanza.Parse<Features>(connection.NextElement());
            if (connection.Features.Bind)
            {
                var bind = new Elements.Bind(connection.Jid.Resource);
                var iq = new XMPPIq(XMPPIq.IqTypes.set);
                iq.Add(bind);
                connection.Query(iq, (bindResult) =>
                {
                    var jid = bindResult.Element(XNamespace.Get(Namespaces.XmppBind) + "bind");
                    if (jid == null)
                    {
                        return;
                    }
                    connection.Jid = new JID(jid.Element(XNamespace.Get(Namespaces.XmppBind) + "jid").Value);
                    if (connection.Features.Session)
                    {
                        var sess = new XElement(XNamespace.Get(Namespaces.XmppSession) + "session");
                        var sessIq = new XMPPIq(XMPPIq.IqTypes.set);
                        sessIq.Add(sess);
                        connection.Query(sessIq, (sessionResponse) => OnSessionStarted(connection));
                    }
                    else
                    {
                        OnSessionStarted(connection);
                    }
                });
                connection.SessionLoop();
            }    
        }
    }
}
