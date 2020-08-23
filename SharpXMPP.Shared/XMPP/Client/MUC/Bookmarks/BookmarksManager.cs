using System.Collections.Generic;
using System.Xml.Linq;
using SharpXMPP.XMPP.Client.Elements;
using SharpXMPP.XMPP.Client.MUC.Bookmarks.Elements;

namespace SharpXMPP.XMPP.Client.MUC.Bookmarks
{
    public class BookmarksManager
    {
        public List<BookmarkedConference> Rooms = new List<BookmarkedConference>();

        public delegate void BookmarksSyncedHandler(XmppConnection sender);

        public event BookmarksSyncedHandler BookmarksSynced = delegate {};

        protected void OnBookmarksSynced(XmppConnection sender)
        {
            BookmarksSynced(sender);
        }

        private readonly XmppConnection connection;

        public BookmarksManager(XmppConnection conn, bool autoAsk = true)
        {
            connection = conn;
            connection.SignedIn += (sender, e) =>
            {
                if (autoAsk)
                {
                    var query = new XMPPIq(XMPPIq.IqTypes.get);
                    var priv = new XElement(XNamespace.Get("jabber:iq:private") + "query",
                        new XElement(XNamespace.Get(Namespaces.StorageBookmarks) + "storage")
                        );
                    query.Add(priv);
                    connection.Query(query, (response) =>
                    {
                        var roomsXML = response.Element(XNamespace.Get("jabber:iq:private") + "query")
                            .Element(XNamespace.Get(Namespaces.StorageBookmarks) + "storage")
                            .Elements(XNamespace.Get(Namespaces.StorageBookmarks) + "conference");
                        foreach (var roomObj in roomsXML)
                        {
                            var room = Stanza.Parse<BookmarkedConference>(roomObj);
                            Rooms.Add(room);
                            if (room.IsAutojoin)
                            {
                                Join(room);
                            }
                        }
                        OnBookmarksSynced(conn);
                    });
                }
            };
        }

        public void Join(BookmarkedConference room)
        {
            var mucPresence = new XMPPPresence(connection.Capabilities)
            {
                To = new JID(string.Format("{0}/{1}", room.JID.BareJid, room.Nick))
            };
            var x = new XElement(XNamespace.Get(Namespaces.MUC) + "x");
            if (room.Password != null)
            {
                x.Add(new XElement(XNamespace.Get(Namespaces.MUC) + "password", room.Password));
            }
            mucPresence.Add(x);
            connection.Send(mucPresence);
        }
    }
}
