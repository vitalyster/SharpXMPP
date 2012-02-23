using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpXMPP.Client;

namespace SharpXMPP.Tests
{
    [TestClass]
    public class XMPPTests
    {
        static void TestJID(JID jid, string user, string domain, string resource)
        {
            Assert.AreEqual(user, jid.User);
            Assert.AreEqual(domain, jid.Domain);
            Assert.AreEqual(resource, jid.Resource);
        }
        [TestMethod]
        public void JIDTests()
        {
            TestJID(new JID("_vt@xmpp.ru/ololo"), "_vt", "xmpp.ru", "ololo");
            TestJID(new JID("icq.jabber.ru"), string.Empty, "icq.jabber.ru", null);
            TestJID(new JID("vasya@icq.org"), "vasya", "icq.org", null);
        }
    }
}
