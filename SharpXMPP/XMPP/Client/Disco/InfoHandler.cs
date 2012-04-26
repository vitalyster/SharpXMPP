using System.Collections.Generic;
using System.Linq;
using SharpXMPP.XMPP.Client.Capabities;
using SharpXMPP.XMPP.Client.Disco.Elements;
using SharpXMPP.XMPP.Client.Elements;

namespace SharpXMPP.XMPP.Client.Disco
{
    public class InfoHandler : PayloadHandler
    {
        private readonly CapabilitiesManager _capabilities;
        public InfoHandler(CapabilitiesManager capabilities)
        {
            _capabilities = capabilities;
        }
        public override bool Handle(XmppConnection connection, Iq element)
        {
            var info = Stanza.Parse<DiscoInfo>(element.Elements().FirstOrDefault());
            if (info != null)
            {
                if (info.Node == string.Empty || info.Node == string.Format("{0}#{1}", _capabilities.Node, _capabilities.OurHash))
                {
                    info.Identity = _capabilities.Identity;
                    info.Features = _capabilities.Features;
                    info.Node = _capabilities.Node;
                    var reply = element.Reply();
                    reply.RemoveNodes();
                    reply.Add(info);
                    connection.Send(reply);
                    return true;
                }

            }
            return false;
        }
    }
}
