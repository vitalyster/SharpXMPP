using System.Collections.Generic;
using System.Linq;
using SharpXMPP.XMPP.Client.Elements;

namespace SharpXMPP.XMPP.Client
{
    public abstract class PayloadHandler
    {
        public abstract bool Handle(XmppConnection sender, Iq element);
    }

    public class IqHandler
    {
        protected readonly XmppConnection Connection;
        public IqHandler(XmppConnection connection)
        {
            Connection = connection;
        }

        public List<PayloadHandler> PayloadHandlers { get; set; }

        public void Handle(Iq element)
        {
            if (PayloadHandlers != null)
            {
                if (PayloadHandlers.Any(handler => handler.Handle(Connection, element)))
                {
                    return;
                }
                HandleError(element);
            }
            HandleError(element);
        }

        public void HandleError(Iq element)
        {
            if (element.IqType == Iq.IqTypes.get || element.IqType == Iq.IqTypes.set)
                Connection.Send(element.Error());
        }
    }
}
