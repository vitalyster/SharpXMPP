using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SharpXMPP.XMPP.Client.Disco.Elements;

namespace SharpXMPP.XMPP.Client.Capabities
{
    public class CapabilitiesManager
    {
        public List<string> Features { get; set; }
        public Identity Identity { get; set; }
        
        public string Node { get; set; }

        public string OurHash { get { return VerifyHash(Identity, Features); } }
        
        public static string VerifyHash(Identity identity, List<string> features )
        {
            var identityString = new StringBuilder();
            identityString.Append(string.Format("{0}/{1}//{2}<", identity.Category, identity.IdentityType, identity.IdentityName));
            features.OrderBy(i => i).ToList().ForEach(f => identityString.Append(string.Format("{0}<", f)));
            SHA1 sha = SHA1.Create();
            var resultBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(identityString.ToString()));
            return Convert.ToBase64String(resultBytes);
        }
    }
}
