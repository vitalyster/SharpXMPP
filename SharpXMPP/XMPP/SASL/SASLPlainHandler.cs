using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpXMPP.XMPP.SASL
{
    public class SASLPlainHandler : SASLHandler
    {
        public  SASLPlainHandler()
        {
            SASLMethod = "PLAIN";
        }

        public override string Initiate()
        {
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(ClientJID.BareJid + '\0' + ClientJID.User + '\0' + Password));
            return token;
        }

        public override string NextChallenge(string previousResponse)
        {
            return string.Empty;
        }
    }
}
