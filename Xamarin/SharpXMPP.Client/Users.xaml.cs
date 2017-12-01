using SharpXMPP.XMPP.Client.Roster.Elements;
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
	public partial class Users : ContentPage
	{
        private readonly XmppClient _xmppconnetction;

        public Users()
        {
            InitializeComponent();

        }

    }
}
