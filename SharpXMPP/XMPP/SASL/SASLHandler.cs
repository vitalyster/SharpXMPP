namespace SharpXMPP.XMPP.SASL
{
    public abstract class SASLHandler
    {
        public string SASLMethod { get; set; }
        public JID ClientJID { get; set; }
        public string Password { get; set; }
        public abstract string Initiate();

        public abstract string NextChallenge(byte[] previousResponse);

        public static SASLHandler Create(string[] availableMethods, JID clientJID, string password)
        {
            return new SASLPlainHandler { ClientJID = clientJID, Password = password};
        }
    }
}
