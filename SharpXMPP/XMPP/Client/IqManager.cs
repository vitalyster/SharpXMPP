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
        protected readonly XmppConnection Connection;
        public IqManager(XmppConnection connection)
        {
            Connection = connection;
        }

        public List<PayloadHandler> PayloadHandlers { get; set; }

        public void Handle(XMPPIq element)
        {
            if (PayloadHandlers != null)
            {
                bool handled = false;
                PayloadHandlers.ForEach( (h) =>
                                             {
                                                 handled |= h.Handle(Connection, element);
                                             });
                if (!handled)
                    HandleError(element);
                else
                {
                    return;
                }
            }
            HandleError(element);
        }

        public void HandleError(XMPPIq element)
        {
            if (element.IqType == XMPPIq.IqTypes.get || element.IqType == XMPPIq.IqTypes.set)
                Connection.Send(element.Error());
        }
    }
}
