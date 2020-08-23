using System.IO;
using System.Xml;
using SharpXMPP.XMPP;

namespace SharpXMPP.NUnit.TestUtils
{
    public class MockedXmppTcpConnection : XmppTcpConnection
    {
        public MockedXmppTcpConnection(Stream inputStream = null, Stream outputStream = null)
            : base("", new JID(), "")
        {
            if (inputStream != null) Reader = XmlReader.Create(inputStream);
            if (outputStream != null) Writer = XmlWriter.Create(outputStream);
        }
    }
}
