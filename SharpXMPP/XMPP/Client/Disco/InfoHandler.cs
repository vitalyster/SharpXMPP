using System.Collections.Generic;
using System.Linq;
using SharpXMPP.XMPP.Client.Disco.Elements;
using SharpXMPP.XMPP.Client.Elements;

namespace SharpXMPP.XMPP.Client.Disco
{
    public class InfoHandler : PayloadHandler
    {
        private readonly List<string> _features;

        public InfoHandler(List<string> features)
        {
            _features = features;
        }
        
        public override bool Handle(XmppConnection connection, Iq element)
        {
            var info = Stanza.Clone<DiscoInfo>(element.Elements().FirstOrDefault());
            if (info != null)
            {
                info.Identity = new Identity
                                    {
                                        IdentityName = "SharpXMPP",
                                        IdentityType = "pc",
                                        Category = "client"
                                    };
                info.Features = _features;
                var reply = element.Reply();
                reply.RemoveNodes();
                reply.Add(info);
                connection.Send(reply);
                return true;
            }
            return false;
        }
    }
}
