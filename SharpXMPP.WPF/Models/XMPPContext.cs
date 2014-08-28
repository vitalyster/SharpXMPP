using SharpXMPP.XMPP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
            modelBuilder.ComplexType<JID>();
            modelBuilder.Entity<Conversation>().HasKey(c => c.JID);
            modelBuilder.Entity<User>().HasKey(u => u.JID);
            modelBuilder.Entity<Account>().HasKey(a => a.JID);
        }
    }

    public class XMPPContextInitializer : DropCreateDatabaseIfModelChanges<XMPPContext>
    {
        protected override void Seed(XMPPContext context)
        {
            var vasya = new User {JID = "vasya@xmpp.ru", Name = "Вася" };
            context.Users.AddOrUpdate(u => u.JID, vasya);
            var message = new Message { From = vasya.JID, Text = "Hello, world!", To = "throwable@jabber.ru"};
            var account = new Account { JID = "_vt@xmpp.ru", Password = "xxx" };

            var conversation = new Conversation
            {
                JID = vasya.JID,
                Name = vasya.Name,
                Account = account,
                Messages = new ObservableCollection<Message>
                {
                    message
                },
                Users = new ObservableCollection<User> {
                    vasya
                }
            };

            context.Accounts.Add(account);
            context.Users.Add(vasya);
            context.SaveChanges();
            context.Messages.Add(message);
            context.SaveChanges();
            context.Conversations.Add(conversation);

            var petya = new User { JID = "petya@xmpp.ru", Name = "Петя" };
            var message2 = new Message { From = petya.JID, Text = "Привет от Пети!", To = "throwable@jabber.ru" };

            var conversation2 = new Conversation
            {
                JID = petya.JID,
                Name = petya.Name,
                Messages = new ObservableCollection<Message>
                {
                    message2
                },
                Users = new ObservableCollection<User> 
                {
                    petya
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
