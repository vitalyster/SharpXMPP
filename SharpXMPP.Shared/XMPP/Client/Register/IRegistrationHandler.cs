using System.Collections.Specialized;
using System.Collections.Generic;

namespace SharpXMPP.XMPP.Client.Register
{
    public interface IRegistrationHandler
    {
        bool OnRegistrationRequest(JID requester, Dictionary<string, string> fields);
        bool OnRegistrationRemove(JID requester);
        string GetInstructions();
        Dictionary<string, string> GetCredentials(JID requester);
    }
}