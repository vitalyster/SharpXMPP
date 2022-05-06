using NUnit.Framework;
using SharpXMPP.XMPP;
using SharpXMPP.XMPP.Client.Disco.Elements;

namespace SharpXMPP.NUnit.XMPP
{
    [TestFixture]
    public class StanzaTests
    {
        [Test]
        public void NullShouldBeParsed()
        {
            Assert.IsNull(Stanza.Parse<Identity>(null));
        }
    }
}
