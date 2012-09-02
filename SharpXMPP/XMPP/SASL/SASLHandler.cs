using System.Collections.Generic;

namespace SharpXMPP.XMPP.SASL
{
    public abstract class SASLHandler
    {
        public string SASLMethod { get; set; }
        public JID ClientJID { get; set; }
        public string Password { get; set; }
        public abstract string Initiate();

        public abstract string NextChallenge(string previousResponse);

        public static SASLHandler Create(List<string> availableMethods, JID clientJID, string password)
        {
            return availableMethods.Contains("PLAIN") ? new SASLPlainHandler { ClientJID = clientJID, Password = password} : null;
        }
    }
}
