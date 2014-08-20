using System.Collections.ObjectModel;
using SharpXMPP.WPF.Helpers;
using SharpXMPP.WPF.Models;
using SharpXMPP.XMPP;
using System.ComponentModel;
using System.Data.Entity;
using System.Windows.Data;

namespace SharpXMPP.WPF.ViewModels
{
    public class ConversationsViewModel : ViewModelBase
    {
        public ConversationsViewModel()
        {
            Db.Conversations.Include("User").Include("Messages").Load();
            Chats = Db.Conversations.Local;
            SendMessageCommand = new DelegateCommand<Conversation>((conversation) =>
            {                
                var message = new Message
                {
                    To = conversation.User.JID,
                    From = new JID("throwable@jabber.ru"),
                    Text = conversation.Draft
                };
                Db.Messages.Add(message);
                Db.SaveChanges();
                conversation.Messages.Add(message);
                conversation.Draft = null;
                Db.SaveChanges();
            }, ()=> true);
        }
        public JID JID { get; set; }
        
        
        public ObservableCollection<Conversation> Chats { get; private set; }

        public DelegateCommand<Conversation> SendMessageCommand { get; set; }
    }
}
