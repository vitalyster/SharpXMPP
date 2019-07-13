using System;

namespace SharpXMPP.Errors
{
    public class XmppConnectionTerminatedException : Exception
    {
        public override string Message => "XMPP connection was terminated by server";
    }
}
