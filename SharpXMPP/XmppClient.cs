using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using SharpXMPP.XMPP;
using SharpXMPP.XMPP.Client.Elements;
using SharpXMPP.XMPP.Client.MUC.Bookmarks;
using SharpXMPP.XMPP.Client;
using SharpXMPP.XMPP.Client.Roster;

namespace SharpXMPP
{
    public class XmppClient : XmppTcpConnection
    {
        public XmppClient(JID jid, string password)
            : base(Namespaces.JabberClient, jid, password)
        {
            SignedIn += (sender, args) =>
            {
                Send(new XMPPPresence());
            };
            bookmarkManager = new BookmarksManager(this);
            rosterManager = new RosterManager(this);	    
        }

        public BookmarksManager bookmarkManager;
        public RosterManager rosterManager;

        protected override int TcpPort
        {
            get { return 5222; }
            set { throw new NotImplementedException(); }
        }

        protected override IEnumerable<IPAddress> HostAddresses
        {
            get
            {
                var addresses = new List<IPAddress>();
                DNS.ResolveXMPPClient(Jid.Domain).ForEach(d => addresses.AddRange(Dns.GetHostAddresses(d.Host)));
                return addresses;
            }
            set { throw new NotImplementedException(); }
        }        
    }
}
