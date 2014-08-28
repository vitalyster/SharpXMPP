using System.Collections.ObjectModel;
using SharpXMPP.WPF.Helpers;
using SharpXMPP.WPF.Models;
using SharpXMPP.XMPP;
using System.ComponentModel;
using System.Data.Entity;
using System.Windows.Data;
using System;
using System.Windows.Threading;

namespace SharpXMPP.WPF.ViewModels
{
    public class ConversationsViewModel : ViewModelBase
    {
        public ConversationsViewModel()
        {
            App.DB.Conversations.Include("Users").Include("Messages").Load();
            _chatsSource = new CollectionViewSource { Source = App.DB.Conversations.Local };
            Chats = _chatsSource.View;
            SendMessageCommand = new DelegateCommand<Conversation>((conversation) =>
            {                
                var message = new Message
                {
                    To = conversation.JID,
                    From = "throwable@jabber.ru",
                    Text = conversation.Draft
                };
                App.DB.Messages.Add(message);
                App.DB.SaveChanges();
                conversation.Messages.Add(message);
                conversation.Draft = null;
                App.DB.SaveChanges();
            }, ()=> true);
        }
        public JID JID { get; set; }

        private CollectionViewSource _chatsSource;
        public ICollectionView Chats { get; private set; }

        public DelegateCommand<Conversation> SendMessageCommand { get; set; }
    }
}
