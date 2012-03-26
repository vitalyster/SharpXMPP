using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SharpXMPP.XMPP
{
    public class Payload : XElement
    {
        public Payload(XName name) : base(name)
        {
            
        }

        public Payload(XElement src) : this(src.Name)
        {
            ReplaceAttributes(src.Attributes());
            ReplaceNodes(src.Nodes());
            Attribute("xmlns").Remove();
        }
    }
}
