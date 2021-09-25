using SharpXMPP.Messaging;
using System;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SharpXMPP.Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Chat : ContentPage
    {
        private readonly ObservableCollection<string> _messages = new ObservableCollection<string>();
        private readonly XMPP.Client.Roster.Elements.RosterItem _roster;
        private readonly XmppClient _connection;

        public Chat(XMPP.Client.Roster.Elements.RosterItem rester, XmppClient xmppconnetction)
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

        private void connection_Message(XmppConnection sender, XMPP.Client.Elements.XMPPMessage e)
        {
            Device.BeginInvokeOnMainThread(() =>
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
