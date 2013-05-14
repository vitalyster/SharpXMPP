using System.Collections.Generic;
using System.Linq;
using SharpXMPP.XMPP.Client.Elements;
using System;

namespace SharpXMPP.XMPP.Client
{
    public abstract class PayloadHandler
    {
        public abstract bool Handle(XmppConnection sender, Iq element);
    }

    public delegate void ResponseHandler(object sender, Iq element);

    public class IqHandler
    {
        protected readonly XmppConnection Connection;
        public IqHandler(XmppConnection connection)
        {
            Connection = connection;
        }

        public Dictionary<string, ResponseHandler> ResponseHandlers { get; set; }

        public List<PayloadHandler> PayloadHandlers { get; set; }

        public void Handle(Iq element)
        {
            if (element.Attribute("type").Value == "result")
            {
                var id = element.Attribute("id");
                if (id == null)
                    return;
                var ProcessResponse = ResponseHandlers[id.Value];
                if (ProcessResponse != null)
                {
                    ProcessResponse(Connection, element);
                }
            }
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
