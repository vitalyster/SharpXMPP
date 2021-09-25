using System;
using System.Xml.Linq;
using SharpXMPP.XMPP.Client.Elements;

namespace SharpXMPP.Messaging
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
            m.SetAttributeValue("type", "chat");
            m.SetAttributeValue("id", msg.MessageID);
            m.SetAttributeValue("to", msg.To);
            m.Text = msg.Text;
            return m;
        }
    }
}
