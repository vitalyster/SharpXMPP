using System.Xml.Linq;

namespace SharpXMPP.Client
{
    public class Message : XElement
    {
        public Message() : base("message")
        {
            
        }
    }
}
