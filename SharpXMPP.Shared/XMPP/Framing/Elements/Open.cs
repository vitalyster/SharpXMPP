using System.Xml.Linq;

namespace SharpXMPP.XMPP.Framing.Elements
{
    public class Open : XElement
    {
        public Open() : base(XNamespace.Get(Namespaces.XmppFraming) + "open")
        {            
        }

        public Open(string to) : this () {
            SetAttributeValue("to", to);
            SetAttributeValue("version", "1.0");
        }

        public string ID
        {
            get
            {
                var id = Attribute("id");
                return id == null ? string.Empty : id.Value;
            }
        }

        public string From
        {
            get
            {
                var from = Attribute("from");
                return from == null ? string.Empty : from.Value;
            }
        }
    }
}
