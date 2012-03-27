using System.Linq;
using System.Xml.Linq;

namespace SharpXMPP.XMPP.Stream.Elements
{
    public class StreamError : Stanza
    {
        public StreamError()
            : base(XNamespace.Get(Namespaces.Streams) + "error")
        {

        }
        public StreamErrorType ErrorType
        {
            get
            {
                var payload = Elements().FirstOrDefault();
                switch (payload.Name.LocalName)
                {
                    case "not-well-formed":
                        return StreamErrorType.NotWellFormed;
                    case "invalid-namespace":
                        return StreamErrorType.InvalidNamespace;
                }
                return StreamErrorType.Unknown;
            }
            set
            {
                XName xname;
                switch(value)
                {
                    case StreamErrorType.InvalidNamespace:
                        xname = XNamespace.Get(Namespaces.StreamErrors) + "invalid-namespace";
                        break;
                    case StreamErrorType.NotWellFormed:
                        xname = XNamespace.Get(Namespaces.StreamErrors) + "not-well-formed";
                        break;
                    default:
                        xname = XNamespace.Get(Namespaces.StreamErrors) + "service-unavailable";
                        break;
                }
                ReplaceNodes(new Stanza(xname));
            }
        }
    }

    public enum StreamErrorType
    {
        Unknown,
        InvalidNamespace,
        NotWellFormed
    }
}
