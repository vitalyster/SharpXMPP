using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SharpXMPP.Stream
{
    [XmlRoot("error", Namespace = Namespaces.Streams)]
    public class Error
    {
        public StreamError ErrorType;
    }

    public enum StreamError
    {
        [XmlElement("invalid-namespace", Namespace = Namespaces.StanzaErrors)]
        InvalidNamespace,
        [XmlElement("not-well-formed", Namespace = Namespaces.StanzaErrors)]
        NotWellFormed
    }
}
