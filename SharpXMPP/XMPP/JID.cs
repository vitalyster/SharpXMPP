using System;

namespace SharpXMPP.XMPP
{
    public class JID : IComparable<JID>, IEquatable<JID>
    {
        public string User { get; set; }
        public string Domain { get; set; }
        public string Resource { get; set; }

        public JID(string jid)
        {
            var domainWithResource = string.Empty;
            var jidParts = jid.Split(new[] { '@' }, 2);
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
            var resourceParts = domainWithResource.Split(new[] { '/' }, 2);
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

        public int CompareTo(JID obj)
        {
            return obj.FullJid.CompareTo(FullJid);
        }

        public override bool Equals(object obj)
        {
            return FullJid.Equals((obj as JID).FullJid);
        }

        public bool Equals(JID obj)
        {
            // If parameter is null, return false.
            if (Object.ReferenceEquals(obj, null))
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, obj))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != obj.GetType())
                return false;
            return obj.FullJid.Equals(FullJid);
        }

        public string ToString()
        {
            return FullJid;
        }
    }
}
