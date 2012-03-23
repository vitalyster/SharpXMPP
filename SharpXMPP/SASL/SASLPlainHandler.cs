using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpXMPP.SASL
{
    public class SASLPlainHandler : SASLHandler
    {
        public  SASLPlainHandler()
        {
            SASLMethod = "PLAIN";
        }

        public override string Initiate()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(ClientJID.BareJid + '\0' + ClientJID.User + '\0' + Password));
        }

        public override string NextChallenge(byte[] previousResponse)
        {
            return string.Empty;
        }
    }
}
