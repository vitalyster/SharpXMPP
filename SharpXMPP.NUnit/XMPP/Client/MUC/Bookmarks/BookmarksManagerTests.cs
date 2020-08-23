using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using NUnit.Framework;
using SharpXMPP.NUnit.TestUtils;
using SharpXMPP.XMPP;
using SharpXMPP.XMPP.Client.Elements;
using SharpXMPP.XMPP.Client.MUC.Bookmarks;
using SharpXMPP.XMPP.Client.MUC.Bookmarks.Elements;

namespace SharpXMPP.NUnit.XMPP.Client.MUC.Bookmarks
{
    [TestFixture]
    public class BookmarksManagerTests
    {
        [Test]
        public static void BookmarksManagerSendsThePasswordToJoin()
        {
            using var stream = new MemoryStream();
            using var connection = new MockedXmppTcpConnection(null, stream);
            var elements = new List<XElement>();
            connection.Element += (_, element) => elements.Add(element.Stanza);

            var conference = new BookmarkedConference
            {
                JID = new JID("test@conference.example.com"),
                Password = "12345"
            };
            var bookmarksManager = new BookmarksManager(connection, false);
            bookmarksManager.Join(conference);

            Thread.MemoryBarrier();
            var joinElement = Stanza.Parse<XMPPPresence>(elements.Single());
            var password = joinElement
                .Element(XNamespace.Get(Namespaces.MUC) + "x")!
                .Element(XNamespace.Get(Namespaces.MUC) + "password")!
                .Value;
            Assert.AreEqual("12345", password);
        }
    }
}
