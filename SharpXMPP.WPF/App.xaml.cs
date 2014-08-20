using System.Collections.Generic;
using System.Threading;
using SharpXMPP.WPF.Models;
using System.Data.Entity;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace SharpXMPP.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        readonly Dictionary<XmppClientConnection, XmlLogger> _conns = new Dictionary<XmppClientConnection, XmlLogger>();
        private XMPPContext db = new XMPPContext();
        protected override void OnStartup(StartupEventArgs e)
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                XmlLanguage.GetLanguage(
                CultureInfo.CurrentCulture.IetfLanguageTag)));

            base.OnStartup(e);
            Database.SetInitializer(new XMPPContextInitializer());
            foreach (var account in db.Accounts)
            {
                var client = new XmppClientConnection(account.JID, account.Password);
                ThreadPool.QueueUserWorkItem((o) => client.Connect());
                _conns.Add(client, new XmlLogger(client, db));
            }
        }

    }
}
