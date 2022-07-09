using SharpXMPP.XMPP;
using SharpXMPP.XMPP.Client.Elements;
using SharpXMPP.XMPP.Client.MUC.Bookmarks;
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
            BookmarkManager = new BookmarksManager(this, autoPresence);
            RosterManager = new RosterManager(this, autoPresence);	    
        }

        public RosterManager RosterManager { get; private set; }

        
    }
}
