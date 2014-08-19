using SharpXMPP.XMPP;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharpXMPP.WPF.Models
{
    public class User
    {
        public int UserID { get; set; }
        [Column(TypeName="nvarchar")]
        public JID JID { get; set; }
        public string Name { get; set; }
    }
}
