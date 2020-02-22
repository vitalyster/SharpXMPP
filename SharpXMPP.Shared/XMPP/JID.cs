using System;

namespace SharpXMPP.XMPP
{
    public partial class JID
    {
        public string User { get; set; }
        public string Domain { get; set; }
        public string Resource { get; set; }

        public JID()
        {

        }

        public JID(string jid)
        {
            if (string.IsNullOrEmpty(jid))
                return;

            var bareJid = string.Empty;
            var jidParts = jid.Split(new[] {'/'}, 2);
            switch (jidParts.Length)
            {
                case 1:
                    bareJid = jidParts[0];
                    break;
                case 2:
                    bareJid = jidParts[0];
                    Resource = jidParts[1];
                    break;
            }

            var bareJidParts = bareJid.Split('@');
            switch (bareJidParts.Length)
            {
                case 1:
                    Domain = bareJidParts[0].ToLower();
                    break;
                case 2:
                    User = bareJidParts[0].ToLower();
                    Domain = bareJidParts[1].ToLower();
                    break;
                default:
                    throw new Exception($"Malformed bare JID: {bareJid}");
            }
        }

        public string BareJid
        {
            get { return string.IsNullOrEmpty(User) ? Domain : string.Format(@"{0}@{1}", User, Domain); }
        }

        public string FullJid
        {
            get { return string.IsNullOrEmpty(Resource) ? BareJid : string.Format(@"{0}/{1}", BareJid, Resource); }
        }

        public string ToString()
        {
            return FullJid;
        }
    }
}
