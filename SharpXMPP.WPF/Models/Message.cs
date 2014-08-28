using SharpXMPP.XMPP;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharpXMPP.WPF.Models
{
    public class Message
    {
        public long MessageID { get; set; }
        public string Text { get; set; }
        public string To { get; set; }
        public string From { get; set; }

        public bool Delivered { get; set; }
        public bool Read { get; set; }
    }
}
