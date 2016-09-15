using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SharpXMPP.XMPP.Client.MUC.Bookmarks.Elements
{
    public class BookmarkedConference : Stanza
    {
        public BookmarkedConference()
            : base(XNamespace.Get(Namespaces.StorageBookmarks) + "conference")
        { }
        public string Name
        {
            get
            {
                var name = Attribute("name");
                return name == null ? string.Empty : name.Value;
            }
        }
        public JID JID
        {
            get
            {
                var name = Attribute("jid");
                return name == null ? null : new JID(name.Value);
            }
        }

        public string Nick
        {
            get
            {
                var nick = Element(XNamespace.Get(Namespaces.StorageBookmarks) + "nick");
                return nick == null ? null : nick.Value;
            }
        }
        public bool IsAutojoin
        {
            get
            {
                var a = Attribute("autojoin");
                return a == null ? false : a.Value == "1" || a.Value == "true";
            }
        }
    }
}

