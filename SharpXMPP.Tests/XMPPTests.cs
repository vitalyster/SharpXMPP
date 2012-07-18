using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpXMPP.XMPP;
using SharpXMPP.XMPP.Client.Disco.Elements;
using SharpXMPP.XMPP.Stream;
using SharpXMPP.XMPP.Stream.Elements;
using SharpXMPP.XMPP.TLS.Elements;
using SuperSocket.ClientEngine;

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
            Assert.AreEqual("vasya@icq.org", new JID("vasya@icq.org").ToString());
            Assert.AreEqual("icq.org", new JID("icq.org").ToString());
            Assert.AreEqual("icq.org/registered", new JID("icq.org/registered").ToString());
        }
        [TestMethod]
        public void DNSTests()
        {
            DNS.ResolveXMPPClient("gmail.com").ForEach(r => Trace.WriteLine(r.Host + ":" + r.Port));
        }

        [TestMethod]
        public void StanzaTests()
        {
            const string xmldata = "<stream:error xmlns:stream=\"http://etherx.jabber.org/streams\"><not-well-formed xmlns=\"urn:ietf:params:xml:ns:xmpp-streams\" /></stream:error>";
            var errorinput = XElement.Parse(xmldata);
            var payload  = Stanza.Parse<StreamError>(errorinput);
            Assert.AreEqual(StreamErrorType.NotWellFormed, payload.ErrorType);

            var error = new StreamError {ErrorType = StreamErrorType.NotWellFormed};
            // Remove all namespace attributes.
            error.DescendantsAndSelf().Attributes().Where(n => n.IsNamespaceDeclaration).Remove();

            // Specify that the namespace will be serialized with a namespace prefix of 'stream'.
            error.Add(new XAttribute(XNamespace.Xmlns + "stream", Namespaces.Streams));
            Assert.AreEqual(payload.ToString(), Stanza.Parse<StreamError>(XElement.Parse(error.ToString())).ToString());
            var bad = Stanza.Parse<StartTLS>(errorinput);
            Assert.IsNull(bad);
        }
        [TestMethod]
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
        }

        [TestMethod]
        public void ClientTest()
        {
            var client = new AsyncTcpSession(new DnsEndPoint("jabber.ru", 5222));
            client.Connected += (sender, args) =>
                                    {
                                        Trace.WriteLine("Connected!");
                                        client.Send(Encoding.UTF8.GetBytes("<stream:stream>"), 0, "<stream:stream>".Length);
                                    };
            client.DataReceived += (sender, args) =>
                                       {
                                           Trace.WriteLine(Encoding.UTF8.GetString(args.Data));
                                       };
            client.Connect();
            while (true) { }
        }
    }
}
