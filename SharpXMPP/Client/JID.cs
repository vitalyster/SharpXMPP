﻿namespace SharpXMPP.Client
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

        public string Resource { get; set; }

        public string BareJid 
        { 
            get { return string.IsNullOrEmpty(User) ? Domain : string.Format("{0}@{1}", User, Domain); }
        }
        
    }
}
