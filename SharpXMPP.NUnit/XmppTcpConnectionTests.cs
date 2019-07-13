using System.IO;
using System.Text;
using NUnit.Framework;
using SharpXMPP.Errors;
using SharpXMPP.NUnit.TestUtils;

namespace SharpXMPP.NUnit
{
    [TestFixture]
    public class XmppTcpConnectionTests
    {
        private const string EmptyXmppStream = @"<?xml version='1.0'?>
<stream:stream from='example.com'
               id='someid'
               xmlns='jabber:client'
               xmlns:stream='http://etherx.jabber.org/streams'
               version='1.0'>
</stream:stream>";

        [Test]
        public void ConnectionShouldStopAfterStreamClosed()
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(EmptyXmppStream)))
            using (var connection = new MockedXmppTcpConnection(stream))
            {
                Assert.Throws<XmppConnectionTerminatedException>(() => connection.NextElement());
            }
        }
    }
}
