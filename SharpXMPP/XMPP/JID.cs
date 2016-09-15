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
            var domainWithResource = string.Empty;
            var jidParts = jid.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            switch (jidParts.Length)
            {
                case 1:
                    domainWithResource = jidParts[0];
                    break;
                case 2:
                    User = jidParts[0].ToLower();
                    domainWithResource = jidParts[1];
                    break;
            }
            var resourceParts = domainWithResource.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            switch (resourceParts.Length)
            {
                case 1:
                    Domain = resourceParts[0].ToLower();
                    break;
                case 2:
                    Domain = resourceParts[0].ToLower();
                    Resource = resourceParts[1];
                    break;
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
