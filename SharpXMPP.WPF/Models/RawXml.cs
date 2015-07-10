using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace SharpXMPP.WPF.Models
{
    public class RawXml
    {
        public long RawXmlID { get; set; }
        public bool IsInput { get; set; }
        [Column(TypeName = "ntext")]
        [MaxLength]
        public string Data { get; set; }
    }
}
