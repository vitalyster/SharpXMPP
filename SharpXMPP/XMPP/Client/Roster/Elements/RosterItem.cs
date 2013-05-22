using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;
using SharpXMPP.XMPP.Client.Elements;
using System.Diagnostics;

namespace SharpXMPP.XMPP.Client.Roster.Elements
{
    [XmlRoot(ElementName = "query", Namespace = "jabber:iq:roster")]
    public class Roster
    {
        [XmlArray(ElementName = "item")]
        public List<RosterItem> Items { get; set; }

        public static void Query(XmppConnection connection)
        {
            var iq = new Iq(Iq.IqTypes.get);
            var query = new Roster();
            iq.Add(Stanza.Serialize<Roster>(query));
            connection.IqTracker.ResponseHandlers.Add(iq.Attribute("id").Value, ProcessResponse);
            connection.Send(iq);
        }

        public static void ProcessResponse(object sender, Iq element)
        {
            var roster = Stanza.Deserialize<Roster>(element.Element(XNamespace.Get("jabber:iq:roster") + "query"));
            foreach (var item in roster.Items)
            {
                Debug.WriteLine(item.BuddyName);
            }
            
        }
    }
    [XmlRoot(ElementName = "item", Namespace = "jabber:iq:roster")]
    public class RosterItem
    {
        [XmlAttribute("jid")]
        public string JID { get; set; }
        [XmlAttribute("name")]
        public string BuddyName { get; set; }
        [XmlAttribute("subscription")]
        public string Subscription { get; set; }
        [XmlAttribute("ask")]
        public string SubscriptionAsk { get; set; }
    } 
   

}
