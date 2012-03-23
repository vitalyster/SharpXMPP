using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpXMPP.Client;

namespace SharpXMPP.SASL
{
    public abstract class SASLHandlerBase
    {
        public string SASLMethod { get; set; }
        public JID ClientJID { get; set; }
        public string Password { get; set; }
        public abstract string Initiate();

        public abstract string NextChallenge(byte[] previousResponse);

        public static SASLHandlerBase Create(string[] availableMethods, JID clientJID, string password)
        {
            return new SASLPlainHandler { ClientJID = clientJID, Password = password};
        }
    }
}
