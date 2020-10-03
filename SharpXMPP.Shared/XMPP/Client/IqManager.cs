using System.Collections.Generic;
using SharpXMPP.XMPP.Client.Elements;

namespace SharpXMPP.XMPP.Client
{
    public abstract class PayloadHandler
    {
        public abstract bool Handle(XmppConnection sender, XMPPIq element);
    }

    public class IqManager
    {
        public List<PayloadHandler> PayloadHandlers { get; } = new List<PayloadHandler>();

        public void Handle(XmppConnection sender, XMPPIq element)
        {
            bool handled = false;
            PayloadHandlers.ForEach( (h) =>
                                         {
                                             handled |= h.Handle(sender, element);
                                         });
            if (!handled)
                HandleError(sender, element);
        }

        public void HandleError(XmppConnection connection, XMPPIq element)
        {
            if (element.IqType == XMPPIq.IqTypes.get || element.IqType == XMPPIq.IqTypes.set)
                connection.Send(element.Error());
        }
    }
}
