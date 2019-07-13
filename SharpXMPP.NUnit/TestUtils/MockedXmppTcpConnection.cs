using System.IO;
using System.Xml;
using SharpXMPP.XMPP;

namespace SharpXMPP.NUnit.TestUtils
{
    public class MockedXmppTcpConnection : XmppTcpConnection
    {
        public MockedXmppTcpConnection(Stream stream) : base("", new JID(), "")
        {
            Reader = XmlReader.Create(stream);
        }
    }
}
