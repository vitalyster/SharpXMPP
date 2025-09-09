using System.Collections.Generic;
using Microsoft.Maui.Controls.Xaml;
using SharpXMPP;
using SharpXMPP.XMPP.Client.Roster.Elements;

namespace SharpXMPP.Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Users : ContentPage
    {
        private readonly XmppClient _xmppconnetction;

        public Users(XmppClient connection)
        {
            InitializeComponent();
            _xmppconnetction = connection;
            _users.ItemsSource = connection.RosterManager.Roster;
        }

        public void RosterSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }
            var rester = (RosterItem)e.SelectedItem;
            _users.SelectedItem = null;
            Navigation.PushModalAsync(new Chat(rester, _xmppconnetction));
        }
    }
}
