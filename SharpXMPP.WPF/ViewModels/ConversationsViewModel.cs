using SharpXMPP.WPF.Helpers;
using SharpXMPP.WPF.Models;
using SharpXMPP.XMPP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace SharpXMPP.WPF.ViewModels
{
    public class ConversationsViewModel : ViewModelBase
    {
        public ConversationsViewModel()
        {
            db.Conversations.Include("User").Include("Messages").Load();
            ChatsSource = new CollectionViewSource { Source = db.Conversations.Local };
            Chats = ChatsSource.View;
            SendMessageCommand = new DelegateCommand<Conversation>((conversation) =>
            {                
                var message = new Message
                {
                    To = conversation.User.JID,
                    From = new JID("throwable@jabber.ru"),
                    Text = conversation.Draft
                };
                db.Messages.Add(message);
                db.SaveChanges();
                conversation.Messages.Add(message);
                conversation.Draft = null;
                db.SaveChanges();
            }, ()=> true);
        }
        public JID JID { get; set; }
        
        
        private CollectionViewSource ChatsSource;

        public ICollectionView Chats { get; set; }

        public DelegateCommand<Conversation> SendMessageCommand { get; set; }
    }
}
