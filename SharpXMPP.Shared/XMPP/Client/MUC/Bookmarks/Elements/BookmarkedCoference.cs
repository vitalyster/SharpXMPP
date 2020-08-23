using System.Xml.Linq;

namespace SharpXMPP.XMPP.Client.MUC.Bookmarks.Elements
{
    public class BookmarkedConference : Stanza
    {
        private const string JidAttributeName = "jid";
        private static readonly XName PasswordElementName = XNamespace.Get(Namespaces.StorageBookmarks) + "password";

        public BookmarkedConference()
            : base(XNamespace.Get(Namespaces.StorageBookmarks) + "conference")
        { }
        public new string Name
        {
            get
            {
                var name = Attribute("name");
                return name == null ? string.Empty : name.Value;
            }
        }
        public JID JID
        {
            get
            {
                var name = Attribute(JidAttributeName);
                return name == null ? null : new JID(name.Value);
            }
            set => SetAttributeValue(JidAttributeName, value);
        }

        public string Nick
        {
            get
            {
                var nick = Element(XNamespace.Get(Namespaces.StorageBookmarks) + "nick");
                return nick == null ? null : nick.Value;
            }
        }

        /// <summary>
        /// Unencrypted string for the password needed to enter a password-protected room. For security reasons, use
        /// of this element is NOT RECOMMENDED by XEP-0048.
        /// </summary>
        public string Password
        {
            get => Element(PasswordElementName)?.Value;
            set
            {
                var passwordElement = Element(PasswordElementName);
                if (value == null)
                {
                    passwordElement?.Remove();
                    return;
                }

                if (passwordElement == null)
                    Add(passwordElement = new XElement(PasswordElementName));

                passwordElement.Value = value;
            }
        }

        public bool IsAutojoin
        {
            get
            {
                var a = Attribute("autojoin");
                return a == null ? false : a.Value == "1" || a.Value == "true";
            }
        }
    }
}

