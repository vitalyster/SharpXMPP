namespace SharpXMPP.Client
{
    public class JID
    {
        public JID(string fromString)
        {
            var at = fromString.IndexOf('@');
            if (at == -1)
            {
                User = string.Empty;
                Domain = fromString.ToLower();
            }
            else
            {
                User = fromString.Substring(0, at).ToLower();
                var slash = fromString.IndexOf('/');
                if (slash == -1)
                {
                    Domain = fromString.Substring(at + 1);
                }
                else
                {
                    Resource = fromString.Substring(slash + 1);
                    Domain = fromString.Substring(at + 1, fromString.Length - User.Length - Resource.Length - 2);
                }
            }
        }

        public string User { get; set; }

        public string Domain { get; set; }

        private string _resource;
        public string Resource 
        { 
            get
            {
                return _resource ?? "SharpXMPP";
            } 
            set
            {
                _resource = value;
            }
        }

        public string BareJid
        {
            get { return string.IsNullOrEmpty(User) ? Domain : string.Format("{0}@{1}", User, Domain); }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            if (User == string.Empty)
            {
                return BareJid;
            }
            return Resource == null ? BareJid : string.Format("{0}/{1}", BareJid, Resource);
        }
    }
}
