using System;
using System.Linq;
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
            var app = Application.Current as App;
            app.Client = new XmppClient(new XMPP.JID(login), pass);
            app.Client.ConnectionFailed += Client_ConnectionFailed;
            app.Client.Element += (s, e) =>
            {
                var direction = e.IsInput ? "<==" : "==>";
                Console.WriteLine($"{direction} {e.Stanza}");
            };
            await app.Client.ConnectAsync();
            IsBusy = false;
            await Navigation.PushModalAsync(new Users(app.Client));
        }

        private void Client_ConnectionFailed(XmppConnection sender, ConnFailedArgs e)
        {
            throw new Exception(e.Message);
        }
    }
}
