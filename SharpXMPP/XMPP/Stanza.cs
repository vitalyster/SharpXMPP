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

        public static T Clone<T>(XElement src) where T : XElement, new()
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
