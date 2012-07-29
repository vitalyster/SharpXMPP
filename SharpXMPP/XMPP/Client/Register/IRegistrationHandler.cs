using System.Collections.Specialized;

namespace SharpXMPP.XMPP.Client.Register
{
    public interface IRegistrationHandler
    {
        bool OnRegistrationRequest(JID requester, StringDictionary fields);
        bool OnRegistrationRemove(JID requester);
        string GetInstructions();
        StringDictionary GetCredentials(JID requester);
    }
}