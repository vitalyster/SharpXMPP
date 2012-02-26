using System.Xml.Linq;

namespace SharpXMPP.Client
{
    public class Presence : XElement
    {
        public Presence() : base("{jabber:client}presence")
        {
            
        }
    }
}
