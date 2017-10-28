using NUnit.Framework;
using SharpXMPP.XMPP.Client.Elements;

namespace SharpXMPP.NUnit
{
    [TestFixture]
    public class XMPPMessageTests
    {
        [Test]
        [TestCase("hello world")]
        [TestCase("<xml>")]
        [TestCase("&lt;")]
        public void TextShouldBeWritable(string text)
        {
            var message = new XMPPMessage {Text = text};
            Assert.AreEqual(text, message.Text);
        }
    }
}
