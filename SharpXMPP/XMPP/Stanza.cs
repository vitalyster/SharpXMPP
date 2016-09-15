using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SharpXMPP.XMPP
{
    public class Stanza : XElement
    {
        public Stanza(XName name) : base(name)
        {
            
        }

        public string ID
        {
            get
            {
                return Attribute("id") == null ? null : Attribute("id").Value;
            }
            set
            {
                SetAttributeValue("id", value);
            }
        }

        public JID From
        {
            get
            {
                return Attribute("from") == null ? null : new JID(Attribute("from").Value);
            }
        }
        public JID To
        {
            get
            {
                return Attribute("to") == null ? null : new JID(Attribute("to").Value);
            }
            set
            {
                SetAttributeValue("to", value.FullJid);
            }
        }

        public static T Parse<T>(XElement src) where T : XElement, new()
        {
            
            var stanza = src as T;
            if (stanza == null)
            {
                stanza = new T();
                stanza.ReplaceAttributes(src.Attributes());
                stanza.ReplaceNodes(src.Nodes());
                if (!src.Name.Equals(stanza.Name))
                    return null;
            }
            return stanza;
        }
    }
}
