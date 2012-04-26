using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpXMPP.XMPP.Client.Elements;

namespace SharpXMPP.XMPP.Client.Disco.Elements
{
    public class ItemsHandler : PayloadHandler
    {
        public override bool Handle(XmppConnection sender, Iq element)
        {
            if (Stanza.Parse<DiscoItems>(element.Elements().FirstOrDefault()) != null)
            {
                sender.Send(element.Reply());
                return true;
            }
            return false;
        }
    }
}
