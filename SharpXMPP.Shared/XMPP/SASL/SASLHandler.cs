using System.Collections.Generic;
using SharpXMPP.XMPP.SASL.Elements;

namespace SharpXMPP.XMPP.SASL
{
    public abstract class SASLHandler
    {
        public string SASLMethod { get; set; }
        public JID ClientJID { get; set; }
        public string Password { get; set; }
        public abstract string Initiate();

        public delegate void AuthenticatedHandler(XmppConnection sender);

        public event AuthenticatedHandler Authenticated = delegate {};

        protected virtual void OnAuthenticated(XmppConnection sender)
        {
            Authenticated(sender);
        }

        public delegate void AuthenticationFailedHandler(XmppConnection sender);

        public event AuthenticationFailedHandler AuthenticationFailed = delegate { };

        protected virtual void OnAuthenticationFailed(XmppConnection sender)
        {
            AuthenticationFailed(sender);
        }

        public abstract string NextChallenge(string previousResponse);

        public static SASLHandler Create(List<string> availableMethods, JID clientJID, string password)
        {
            // NOTE: Made SASLSCRAM as default because used free xmpp servers as https://chat.chinwag.im
            // they dont send any availableMethods
            // return availableMethods.Contains("SCRAM-SHA-1") ? new SASLSCRAM { ClientJID = clientJID, Password = password} : null;
            return new SASLSCRAM { ClientJID = clientJID, Password = password };
        }

        public void Start(XmppConnection connection)
        {
            var auth = new SASLAuth();
            auth.SetAttributeValue("mechanism", SASLMethod);
            auth.SetValue(Initiate());
            connection.Send(auth);
            var authResponse = connection.NextElement();
            var nextResponse = string.Empty;
            while ((nextResponse = NextChallenge(authResponse.Value)) != "")
            {
                if (nextResponse == "error")
                {
                    OnAuthenticationFailed(connection);
                    return;
                }
                var response = new SASLResponse();
                response.SetValue(nextResponse);
                connection.Send(response);
                authResponse = connection.NextElement();
            }
            OnAuthenticated(connection);
        }
    }
}
