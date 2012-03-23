using System.Xml.Linq;
using System.Xml.Serialization;

namespace SharpXMPP.XMPP.Stream.Elements
{
    [XmlRoot("error", Namespace = Namespaces.Streams)]
    public class Error : XElement
    {
        public Error()
            : base(XNamespace.Get(Namespaces.Streams) + "error")
        {

        }
        public StreamErrorType ErrorType;
    }

    public enum StreamErrorType
    {
        [XmlElement("invalid-namespace", Namespace = Namespaces.StanzaErrors)]
        InvalidNamespace,
        [XmlElement("not-well-formed", Namespace = Namespaces.StanzaErrors)]
        NotWellFormed
    }
}
