using SharpXMPP.XMPP;
using System.ComponentModel.DataAnnotations.Schema;

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
