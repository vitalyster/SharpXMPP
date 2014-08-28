using System.Collections.Generic;
using System.Threading;
using SharpXMPP.WPF.Models;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Diagnostics;
using SharpXMPP.XMPP;

namespace SharpXMPP.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        XmppClient _conn;
        XmlLogger _logger;
        private static XMPPContext _db;
        public static XMPPContext DB
        {
            get
            {
                if (_db == null)
                {
                    _db = new XMPPContext();
                }
                return _db;
            }
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                XmlLanguage.GetLanguage(
                CultureInfo.CurrentCulture.IetfLanguageTag)));

            base.OnStartup(e);
            Database.SetInitializer(new XMPPContextInitializer());
            var account = DB.Accounts.ToList().First();
            _conn = new XmppClient(new JID(account.JID), account.Password);
            _logger = new XmlLogger(_conn, DB);
            _conn.bookmarkManager.BookmarksSynced += (conn) =>
            {
                foreach (var room in _conn.bookmarkManager.rooms)
                {
                    var exist = DB.Conversations.FirstOrDefault(r => r.JID == room.JID.FullJid);
                    if (exist == null)
                    {
                        DB.Conversations.Add(new Conversation
                        {
                            JID = room.JID.FullJid,
                            Name = room.Name
                        });
                    }
                }
                DB.SaveChanges();

            };
            _conn.Connect();
            Trace.Listeners.Add(new ConsoleTraceListener());
        }

    }
}
