using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using SharpXMPP.WindowsPhone;
using SharpXMPP.XMPP;

namespace SharpXMPP.MangoClient
{
    public partial class MainPage : PhoneApplicationPage
    {
        private XmppAsyncSocketConnection connection;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            connection = new XmppAsyncSocketConnection(new JID("jid@server.com/ololo"), "password") { InitialPresence = true };
            connection.SignedIn += (sender, args) => MessageBox.Show("Signed in: " + args.Jid);
            ThreadPool.QueueUserWorkItem((o) => connection.Connect());
            
        }
    }
}