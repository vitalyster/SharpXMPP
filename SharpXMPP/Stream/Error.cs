using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SharpXMPP.Stream
{
    [XmlRoot("error", Namespace = "http://etherx.jabber.org/streams")]
    public class Error
    {
        public StreamError ErrorType;
    }

    public enum StreamError
    {
        [XmlElement("invalid-namespace", Namespace = "urn:ietf:params:xml:ns:xmpp-streams")]
        InvalidNamespace,
        [XmlElement("not-well-formed", Namespace = "urn:ietf:params:xml:ns:xmpp-streams")]
        NotWellFormed
    }
}
