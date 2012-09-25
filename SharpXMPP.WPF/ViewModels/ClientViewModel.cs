using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Controls;
using Caliburn.Micro;
using SharpXMPP.XMPP;

namespace SharpXMPP.WPF.ViewModels
{
    [Export(typeof(ClientViewModel))]
    public class ClientViewModel : PropertyChangedBase
    {
        private string _jid;
        public string JID
        {
            get { return _jid; }
            set { _jid = value; NotifyOfPropertyChange(() => JID); NotifyOfPropertyChange(() => CanClientConnect); }
        }

        public void SignIn(object PasswordBox)
        {
            var ps = PasswordBox as PasswordBox;
            var password = ps.SecurePassword;
            Client = new XmppClientConnection(new JID(JID), password);
            Client.Element += (sender, args) => Execute.OnUIThread(() => XmlLog.Add(args.Stanza.ToString()));
            Task.Factory.StartNew(() => Client.Connect());
        }

        public bool CanClientConnect
        {
            get { return (!string.IsNullOrWhiteSpace(JID)); }
        }

        public XmppClientConnection Client;
        private IObservableCollection<string> _xmlLog;
        public IObservableCollection<string> XmlLog
        {
            get { return _xmlLog ?? (_xmlLog = new BindableCollection<string>()); }
            set { _xmlLog = value; NotifyOfPropertyChange(() => XmlLog); }
        }
    }
}
