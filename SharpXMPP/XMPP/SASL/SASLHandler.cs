using System.Collections.Generic;
using System.Security;

namespace SharpXMPP.XMPP.SASL
{
    public abstract class SASLHandler
    {
        public string SASLMethod { get; set; }
        public JID ClientJID { get; set; }
        public SecureString Password { get; set; }
        public abstract string Initiate();

        public abstract string NextChallenge(string previousResponse);

        public static SASLHandler Create(List<string> availableMethods, JID clientJID, SecureString password)
        {
            return availableMethods.Contains("DIGEST-MD5") ? new SASLDigestMd5 { ClientJID = clientJID, Password = password} : null;
        }
    }
}
