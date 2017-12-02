using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SharpXMPP.Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Chat : ContentPage
    {
        private readonly ObservableCollection<string> _messages = new ObservableCollection<string>();
        private readonly XMPP.Client.Roster.Elements.RosterItem _rester;
        private readonly XmppClient _xmppconnetction;

        public Chat(XMPP.Client.Roster.Elements.RosterItem rester, SharpXMPP.XmppClient xmppconnetction)
        {
            InitializeComponent();
            _rester = rester;
            _xmppconnetction = xmppconnetction;

            _msg.ItemsSource = _messages;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _xmppconnetction.Message += _xmppconnetction_Message;
        }

        protected override void OnDisappearing()
        {
            _xmppconnetction.Message -= _xmppconnetction_Message;
            base.OnDisappearing();
        }

        private void _xmppconnetction_Message(SharpXMPP.XmppConnection sender, SharpXMPP.XMPP.Client.Elements.XMPPMessage e)
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
            _xmppconnetction.Send(new XMPP.Client.Elements.XMPPMessage
            {
                Text = text,
                To = new XMPP.JID(_rester.JID)
            });
            _newMsg.Text = string.Empty;
        }
    }
}
