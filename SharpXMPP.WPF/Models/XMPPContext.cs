using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration.Conventions;
using SharpXMPP.Messaging;
using SharpXMPP.XMPP;

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
            modelBuilder.Entity<Message>().HasKey(m => m.MessageID);
        }
    }

    public class XMPPContextInitializer : DropCreateDatabaseIfModelChanges<XMPPContext>
    {
        protected override void Seed(XMPPContext context)
        {
            var account = new Account { JID = "_vt@xmpp.ru", Password = "secret" };
            context.Accounts.AddOrUpdate(a => a.JID, account);
            context.SaveChanges();           
            
            base.Seed(context);
        }        
    }    
}
