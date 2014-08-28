using SharpXMPP.XMPP;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharpXMPP.WPF.Models
{
    public class User
    {
        public string JID { get; set; }
        public string Name { get; set; }
    }
}
