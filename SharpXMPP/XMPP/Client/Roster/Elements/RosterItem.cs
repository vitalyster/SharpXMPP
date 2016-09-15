using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SharpXMPP.XMPP.Client.Roster.Elements
{
    public class RosterItem : XElement
    {
        public RosterItem()
            : base(XNamespace.Get(Namespaces.JabberRoster) + "item")
        {
            
        }

        public string JID 
        { 
            get 
            {
                return Attribute("jid") == null ? null : Attribute("jid").Value;
            }    
        }
        public string Name
        {
            get
            {
                return Attribute("name") == null ? JID : Attribute("name").Value;
            }
        }
        
    }
}