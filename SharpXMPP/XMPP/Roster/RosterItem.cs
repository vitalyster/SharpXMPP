using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpXMPP.XMPP.Roster
{
    [XmlRoot(ElementName = "query", Namespace = "jabber:iq:roster")]
    public class Roster
    {
        public Roster()
        {
            Items = new List<RosterItem>();
        }
        [XmlElement(ElementName = "item")]
        public List<RosterItem> Items { get; set; }
    }
    [XmlRoot("item")]
    public class RosterItem
    {
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
