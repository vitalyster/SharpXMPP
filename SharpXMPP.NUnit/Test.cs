using NUnit.Framework;
using System.Linq;
using System.Xml.Linq;
using SharpXMPP.XMPP;
using SharpXMPP.XMPP.Stream.Elements;
using SharpXMPP.XMPP.TLS.Elements;
using SharpXMPP.XMPP.Client.Disco.Elements;
using System.Collections.Generic;
using SharpXMPP.XMPP.Client.MUC.Bookmarks.Elements;

namespace SharpXMPP.NUnit
{
    [TestFixture ()]
    public class Test
    {
        static void TestJID(JID jid, string user, string domain, string resource)
        {
            Assert.AreEqual(user, jid.User);
            Assert.AreEqual(domain, jid.Domain);
            Assert.AreEqual(resource, jid.Resource);
        }
        [Test()]
        public void JIDTests()
        {
            TestJID(new JID("_vt@xmpp.ru/ololo"), "_vt", "xmpp.ru", "ololo");
            TestJID(new JID("icq.jabber.ru"), null, "icq.jabber.ru", null);
            TestJID(new JID("vasya@icq.org"), "vasya", "icq.org", null);
            TestJID(new JID("user@domain.org/nickname@domain.org"), "user", "domain.org", "nickname@domain.org");
            TestJID(new JID("user@domain.org/nick@NAME@domain.org"), "user", "domain.org", "nick@NAME@domain.org");
            TestJID(new JID("user@domain.org/nickname =/\\__/\\="), "user", "domain.org", "nickname =/\\__/\\=");
            TestJID(new JID("user@domain.org/user/@"), "user", "domain.org", "user/@");
            TestJID(new JID("user@domain.org/@@@@"), "user", "domain.org", "@@@@");
            TestJID(new JID(""), null, null, null);
            TestJID(new JID("user@domain@wrong"), null, null, null);
            TestJID(new JID("user@domain@wrong/resource"), null, null, "resource");
            Assert.AreEqual("vasya@icq.org", new JID("vasya@icq.org").ToString());
            Assert.AreEqual("icq.org", new JID("icq.org").ToString());
            Assert.AreEqual("icq.org/registered", new JID("icq.org/registered").ToString());
            Assert.AreEqual(new JID("vasya@msn.com").FullJid, new JID("VASYA@msn.com").FullJid);
            Assert.AreNotEqual(new JID("vasya@msn.com/QQ").FullJid, new JID("VASYA@msn.com/qq").FullJid);
        }

        [Test()]
        public void StanzaTests()
        {
            const string xmldata = "<stream:error xmlns:stream=\"http://etherx.jabber.org/streams\"><not-well-formed xmlns=\"urn:ietf:params:xml:ns:xmpp-streams\" /></stream:error>";
            var errorinput = XElement.Parse(xmldata);
            var payload = Stanza.Parse<StreamError>(errorinput);
            Assert.AreEqual(StreamErrorType.NotWellFormed, payload.ErrorType);

            var error = new StreamError { ErrorType = StreamErrorType.NotWellFormed };
            // Remove all namespace attributes.
            error.DescendantsAndSelf().Attributes().Where(n => n.IsNamespaceDeclaration).Remove();

            // Specify that the namespace will be serialized with a namespace prefix of 'stream'.
            error.Add(new XAttribute(XNamespace.Xmlns + "stream", Namespaces.Streams));
            Assert.AreEqual(payload.ToString(), Stanza.Parse<StreamError>(XElement.Parse(error.ToString())).ToString());
            var bad = Stanza.Parse<StartTLS>(errorinput);
            Assert.IsNull(bad);
        }
        [Test()]
        public void IdentityTests()
        {
            var info = new DiscoInfo
            {
                Identity = new Identity
                {
                    IdentityName = "SharpXMPP",
                    IdentityType = "pc",
                    Category = "client"
                },
                Features = new List<string>
                                              {
                                                  Namespaces.DiscoInfo
                                              }
            };
            var cf = new XElement(XNamespace.Get("storage:bookmarks") + "conference");
            cf.SetAttributeValue("jid", "to@to.ti");
            cf.SetAttributeValue("name", "lalallaa");
            cf.SetAttributeValue("autojoin", "false");
            var room = Stanza.Parse<BookmarkedConference>(cf);
            Assert.IsFalse(room.IsAutojoin);
            var cf2 = new XElement(XNamespace.Get("storage:bookmarks") + "conference");
            cf2.SetAttributeValue("jid", "to@to.ti");
            cf2.SetAttributeValue("name", "lalallaa");
            cf2.SetAttributeValue("autojoin", "1");
            var room2 = Stanza.Parse<BookmarkedConference>(cf2);
            Assert.IsTrue(room2.IsAutojoin);
        }
    }
}

