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
        public XmppClient(JID jid, string password, bool autoPresence = true)
            : base(Namespaces.JabberClient, jid, password)
        {
            SignedIn += (sender, args) =>
            {
                if (autoPresence)
                {
                    Send(new XMPPPresence());
                }
            };
            _bookmarkManager = new BookmarksManager(this, autoPresence);
            _rosterManager = new RosterManager(this, autoPresence);	    
        }

        private BookmarksManager _bookmarkManager;
        private RosterManager _rosterManager;

        
    }
}
