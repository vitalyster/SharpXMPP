using SharpXMPP.XMPP.Client.Elements;
using SharpXMPP.XMPP.Client.MUC.Bookmarks.Elements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SharpXMPP.XMPP.Client.MUC.Bookmarks
{
    public class BookmarksManager
    {
        XmppConnection _conn;

        public List<BookmarkedConference> rooms = new List<BookmarkedConference>();

        public delegate void BookmarksSyncedHandler(XmppConnection sender);

        public event BookmarksSyncedHandler BookmarksSynced = delegate {};

        protected void OnBookmarksSynced(XmppConnection sender)
        {
            BookmarksSynced(sender);
        }

        public BookmarksManager(XmppConnection conn)
        {
            _conn = conn;
            _conn.SignedIn += (sender, e) => 
            {
                var query = new Iq(Iq.IqTypes.get);
                var priv = new XElement(XNamespace.Get("jabber:iq:private") + "query", 
                    new XElement(XNamespace.Get(Namespaces.StorageBookmarks) + "storage")
                    );
                query.Add(priv);
                _conn.Query(query, (response) =>
                {
                    var roomsXML = response.Element(XNamespace.Get("jabber:iq:private") + "query")
                        .Element(XNamespace.Get(Namespaces.StorageBookmarks) + "storage")
                        .Elements(XNamespace.Get(Namespaces.StorageBookmarks) + "conference");
                    foreach (var room in roomsXML)
                    {
                        rooms.Add(Stanza.Parse<BookmarkedConference>(room));   
                    }
                    OnBookmarksSynced(_conn);
                });

            };
        }
    }
}
