using SharpXMPP.XMPP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SharpXMPP.WPF.Models
{
    public class Message
    {
        public long MessageID { get; set; }
        public string Text { get; set; }
        [Column(TypeName = "nvarchar")]
        public JID To { get; set; }
        [Column(TypeName = "nvarchar")]
        public JID From { get; set; }

        public bool Delivered { get; set; }
        public bool Read { get; set; }
    }
}
