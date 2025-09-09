using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.Xaml;
using SharpXMPP;
using SharpXMPP.XMPP.Client.Roster.Elements;
using SharpXMPP.Messaging;
using SharpXMPP.XMPP.Client.Elements;


namespace SharpXMPP.Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Chat : ContentPage
    {
        private readonly ObservableCollection<string> _messages = new ObservableCollection<string>();
        private readonly RosterItem _roster;
        private readonly XmppClient _connection;

        public Chat(RosterItem rester, XmppClient xmppconnetction)
        {
            InitializeComponent();
            _roster = rester;
            _connection = xmppconnetction;

            _msg.ItemsSource = _messages;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _connection.Message += connection_Message;
        }

        protected override void OnDisappearing()
        {
            _connection.Message -= connection_Message;
            base.OnDisappearing();
        }

        private void connection_Message(XmppConnection sender, XMPPMessage e)
        {
            Dispatcher.Dispatch(() =>
                _messages.Add(e.Text)
            );
        }

        private void Send_Clicked(object sender, EventArgs args)
        {
            string text = _newMsg.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            _messages.Add(text);
            _connection.Send(Message.toXMPP(new Message
            {
                Text = text,
                To = _roster.JID
            }));
            _newMsg.Text = string.Empty;
        }
    }
}
