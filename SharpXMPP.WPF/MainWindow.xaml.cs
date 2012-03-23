using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpXMPP.Client;

namespace SharpXMPP.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private XmppTcpClientConnection client;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            client = new XmppTcpClientConnection(new JID(textBox1.Text), passwordBox1.Password );
            client.Element += (o, args) => Dispatcher.Invoke((Action)(() => listBox1.Items.Add(args.Stanza.ToString())));
            ThreadPool.QueueUserWorkItem((o) => client.MainLoop());
        }
    }
}
