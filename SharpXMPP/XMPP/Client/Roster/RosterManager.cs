using SharpXMPP.XMPP.Client.Elements;
using SharpXMPP.XMPP.Client.Roster.Elements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SharpXMPP.XMPP.Client.Roster
{
    public class RosterManager
    {
        public delegate void RosterUpdatedHandler(XmppConnection sender);

        public event RosterUpdatedHandler RosterUpdated = delegate { };

        protected void OnRosterUpdated(XmppConnection sender)
        {
            RosterUpdated(sender);
        }
        public ObservableCollection<RosterItem> Roster {get;set;}
        public RosterManager(XmppConnection conn)
        {
            Roster = new ObservableCollection<RosterItem>();
            conn.SignedIn += (sender, e) =>
                {
                    var query = new XMPPIq(XMPPIq.IqTypes.get);
                    query.Add(new XElement(XNamespace.Get(Namespaces.JabberRoster) + "query"));
                    conn.Query(query, (response) =>
                        {
                            var roster = response.Element(XNamespace.Get(Namespaces.JabberRoster) + "query")
                                .Elements(XNamespace.Get(Namespaces.JabberRoster) + "item");
                            foreach (var item in roster)
                            {
                                Roster.Add(Stanza.Parse<RosterItem>(item));
                            }
                            OnRosterUpdated(conn);
                        });
                };
        }
    }
}
