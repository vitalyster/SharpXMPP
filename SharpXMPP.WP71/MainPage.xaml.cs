using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace SharpXMPP.WP71
{
    public partial class MainPage : PhoneApplicationPage
    {
        XmppWebSocketConnection conn;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            conn = new XmppWebSocketConnection(new XMPP.JID("_vt@xmpp.ru/ph"), "", "ws://127.0.0.1:8080/");
            conn.Element += new XmppConnection.ElementHandler(conn_Element);
            conn.ConnectionFailed += new XmppConnection.ConnectionFailedHandler(conn_ConnectionFailed);
            conn.Connect();
        }

        void conn_ConnectionFailed(object sender, ConnFailedArgs e)
        {
            Dispatcher.BeginInvoke(() => ContentPanel.Items.Add("Stream error: " + e.Message));
        }

        void conn_Element(object sender, ElementArgs e)
        {
            Dispatcher.BeginInvoke(() => ContentPanel.Items.Add(e.Stanza));
        }
    }
}