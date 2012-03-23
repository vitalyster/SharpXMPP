using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SharpXMPP.XMPP.Elements;

namespace SharpXMPP
{
    public abstract class IqHandlerBase
    {
        protected readonly XmppConnection Connection;

        protected IqHandlerBase(XmppConnection connection)
        {
            Connection = connection;
        }
        public IqHandlerBase NextHandler { get; set; }

        public abstract void Handle(Iq element);
    }

    public class IqHandler : IqHandlerBase
    {
        public IqHandler(XmppConnection connection) : base(connection)
        {
        }

        public override void Handle(Iq element)
        {
            if (NextHandler != null) return;
            if (element.IqType == Iq.IqTypes.get || element.IqType == Iq.IqTypes.set)
                Connection.Send(element.Error());
        }
    }
}
