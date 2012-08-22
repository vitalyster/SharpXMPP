using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using Caliburn.Micro;
using SharpXMPP.XMPP;

namespace SharpXMPP.WPF.ViewModels
{
    public class ClientViewModel : PropertyChangedBase
    {
        public ClientViewModel()
        {
            XmlLog = new ObservableCollection<XElement>();
        }
        private string _jid;
        public string JID
        {
            get { return _jid; }
            set { _jid = value; NotifyOfPropertyChange(() => JID); NotifyOfPropertyChange(() => CanClientConnect); }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }

        public void ClientConnect()
        {
            Client = new XmppClientConnection(new JID(JID), Password);
            Client.Element += (sender, args) => XmlLog.Add(args.Stanza);
            Client.Connect();
        }

        public bool CanClientConnect
        {
            get { return (!string.IsNullOrWhiteSpace(JID)); }
        }

        public XmppClientConnection Client;
        public ObservableCollection<XElement> XmlLog { get; set; }
    }
}
