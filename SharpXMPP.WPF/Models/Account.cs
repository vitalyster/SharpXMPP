using SharpXMPP.XMPP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SharpXMPP.WPF.Models
{
    public class Account
    {
        public int AccountID { get; set; }
        [Column(TypeName="nvarchar")]
        public JID JID { get; set; }
        public string Password { get; set; }
    }
}
