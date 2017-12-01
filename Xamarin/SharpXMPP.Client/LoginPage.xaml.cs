using SharpXMPP;
using SharpXMPP.XMPP.Client.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SharpXMPP.Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }


        public async void Login_Click(object sender, EventArgs args)
        {
            var login = _login.Text;
            var pass = _pass.Text;

            IsBusy = true;
            var client = new XmppClient(new SharpXMPP.XMPP.JID(login), pass);
            client.ConnectionFailed += Client_ConnectionFailed;
            await client.ConnectAsync();
            IsBusy = false;
            await Navigation.PushModalAsync(new Users(client));
        }

        private void Client_ConnectionFailed(XmppConnection sender, ConnFailedArgs e)
        {
            throw new Exception(e.Message);
        }
    }
}
