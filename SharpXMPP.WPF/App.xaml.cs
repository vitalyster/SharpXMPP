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
using System;

namespace SharpXMPP.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static XmppClient _conn;
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
            _conn.ConnectionFailed += (sender, args) =>
                {
                    Trace.WriteLine("Disconnected: " + args.Message);
                };
            _conn.Element += (sender, el) =>
            {
                Trace.WriteLine(el.Stanza.ToString());
            };
            _conn.Message += (conn, message) =>
                {
                    if (message.Text == null) return;
                    if (string.IsNullOrEmpty(message.ID))
                    {
                        message.ID = Guid.NewGuid().ToString();
                    }
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var exist = DB.Messages.FirstOrDefault(m => m.MessageID == message.ID);
                        if (exist == null)
                        {
                            var user = DB.Users.FirstOrDefault(u => u.JID == message.From.BareJid);
                            if (user == null)
                            {
                                user = new User { JID = message.From.BareJid, Name = message.From.FullJid };
                                DB.Entry(user).State = EntityState.Added;
                            }
                            var conversation = DB.Conversations.FirstOrDefault(c => c.JID == user.JID);
                            if (conversation == null)
                            {
                                conversation = new Conversation { JID = user.JID, Name = user.Name };
                                DB.Entry(conversation).State = EntityState.Added;
                                conversation.Users.Add(user);
                            }
                            conversation.Messages.Add(
                                new Message
                                {
                                    From = message.From.FullJid,
                                    MessageID = message.ID,
                                    To = message.To.FullJid,
                                    Text = message.Text
                                });
                            DB.SaveChanges();
                        }
                    }));
                };
            _conn.BookmarkManager.BookmarksSynced += (conn) =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        foreach (var room in _conn.BookmarkManager.Rooms)
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
                    }));

            };
            DateTime lastUpdateTime = DateTime.Now;
            _conn.RosterManager.RosterUpdated += (conn) =>
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            foreach (var user in _conn.RosterManager.Roster)
                            {
                                var exist = DB.Users.FirstOrDefault(u => u.JID == user.JID);
                                if (exist == null)
                                {
                                    DB.Users.Add(new User
                                    {
                                        JID = user.JID,
                                        Name = user.Name
                                    });
                                }
                            }
                            if ((lastUpdateTime - DateTime.Now).TotalMilliseconds > 500)
                            {
                                DB.SaveChanges();
                                lastUpdateTime = DateTime.Now;
                            }

                        }));
                };
            _conn.ConnectAsync();
            // ThreadPool.QueueUserWorkItem((o) => _conn.Connect());
            //Trace.Listeners.Add(new ConsoleTraceListener());
        }

    }
}
