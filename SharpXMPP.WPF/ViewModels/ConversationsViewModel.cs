using System.ComponentModel;
using System.Data.Entity;
using System.Windows.Data;
using SharpXMPP.WPF.Helpers;
using SharpXMPP.WPF.Models;
using SharpXMPP.XMPP;

namespace SharpXMPP.WPF.ViewModels
{
    public class ConversationsViewModel : ViewModelBase
    {
        public ConversationsViewModel()
        {
            App.DB.Conversations.Include("Users").Include("Messages").Load();
            _chatsSource = new CollectionViewSource { Source = App.DB.Conversations.Local };
            Chats = _chatsSource.View;
            _usersSource = new CollectionViewSource { Source = App.DB.Users.Local };
            Users = _usersSource.View;
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
                App._conn.Send(Message.toXMPP(message));
            }, ()=> true);
        }
        public JID JID { get; set; }

        private CollectionViewSource _chatsSource;
        public ICollectionView Chats { get; private set; }

        private CollectionViewSource _usersSource;
        public ICollectionView Users { get; private set; }

        public DelegateCommand<Conversation> SendMessageCommand { get; set; }
    }
}
