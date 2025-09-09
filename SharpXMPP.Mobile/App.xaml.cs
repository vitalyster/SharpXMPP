using Microsoft.Maui.Controls;
using SharpXMPP;

namespace SharpXMPP.Client;

public partial class App : Application
{
    public XmppClient Client { get; set; }

    public App()
    {
        InitializeComponent();

        MainPage = new NavigationPage(new LoginPage());
    }
}
