using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SharpXMPP.XMPP.Client.Roster.Elements
{
    public class Roster : Stanza
    {
        public Roster() : base(XNamespace.Get("jabber:iq:roster") + "query")
        {
            Items = new RosterItem[1];
        }

        [XmlArrayItem(ElementName = "item", Namespace = "jabber:iq:roster")]
        public RosterItem[] Items { get; set; }
    }
    public class RosterItem : Stanza
    {
        public RosterItem() : base(XNamespace.Get("jabber:iq:roster") + "item")
        {
            
        }

        [XmlAttribute("jid")]
        public string JID { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("subscription")]
        public string Subscription { get; set; }
        [XmlAttribute("ask")]
        public string SubscriptionAsk { get; set; }
    }
}
