using SharpXMPP.XMPP;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SharpXMPP.WPF.Models
{
    public class XMPPContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public DbSet<Conversation> Conversations { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<RawXml> Log { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    public class XMPPContextInitializer : DropCreateDatabaseAlways<XMPPContext>
    {
        protected override void Seed(XMPPContext context)
        {
            var vasya = new User {JID = new JID("vasya@xmpp.ru"), Name = "Вася" };
            var message = new Message { From = vasya.JID, Text = "Hello, world!", To = new JID("throwable@jabber.ru")};
            var account = new Account { JID = new JID("throwable@jabber.ru"), Password = "IOException" };

            var conversation = new Conversation
            {
                Account = account,
                User = vasya,                
                Messages = new ObservableCollection<Message>
                {
                    message
                }
            };
            
            context.Accounts.Add(account);
            context.Users.Add(vasya);
            context.SaveChanges();
            context.Messages.Add(message);
            context.SaveChanges();
            context.Conversations.Add(conversation);

            var petya = new User { JID = new JID("petya@xmpp.ru"), Name = "Петя" };
            var message2 = new Message { From = petya.JID, Text = "Привет от Пети!", To = new JID("throwable@jabber.ru") };

            var conversation2 = new Conversation
            {
                User = petya,
                Messages = new ObservableCollection<Message>
                {
                    message2
                }
            };

            context.Users.Add(petya);
            context.SaveChanges();
            context.Messages.Add(message2);
            context.SaveChanges();
            context.Conversations.Add(conversation2);

            
            base.Seed(context);
        }
    }
}
