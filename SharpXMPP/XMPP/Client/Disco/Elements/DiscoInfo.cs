using System.Xml.Linq;

namespace SharpXMPP.XMPP.Client.Disco.Elements
{
    class DiscoInfo : Stanza
    {
        public DiscoInfo() : base(XNamespace.Get(Namespaces.DiscoInfo) + "query")
        {
            
        }
    }
}
