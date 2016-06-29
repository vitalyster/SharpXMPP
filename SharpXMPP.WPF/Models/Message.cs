using System;
using System.Xml.Linq;
using SharpXMPP.XMPP.Client.Elements;

namespace SharpXMPP.WPF.Models
{
    public class Message
    {
        public string MessageID { get; set; }
        public string Text { get; set; }
        public string To { get; set; }
        public string From { get; set; }

        public bool Delivered { get; set; }
        public bool Read { get; set; }

        public Message() { }

        public Message(string id = "")
        {
            MessageID = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;
        }

        public static XMPPMessage toXMPP(Message msg)
        {
            var m = new XMPPMessage();
            m.SetAttributeValue(XNamespace.Get(Namespaces.JabberClient) + "type", "chat");
            m.SetAttributeValue(XNamespace.Get(Namespaces.JabberClient) + "id", msg.MessageID);
            m.SetAttributeValue(XNamespace.Get(Namespaces.JabberClient) + "to", msg.To);
            var body = new XElement(XNamespace.Get(Namespaces.JabberClient) + "body");
            body.Value = msg.Text;
            m.Add(body);
            return m;
        }
    }
}
