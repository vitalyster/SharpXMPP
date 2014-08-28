using SharpXMPP.WPF.ViewModels;
using SharpXMPP.XMPP;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharpXMPP.WPF.Models
{
    public class Conversation : ViewModelBase
    {
        public string JID {get;set;}
        public string Name { get; set; }
        public Account Account { get; set; }
        public virtual ObservableCollection<User> Users { get; set; }
        public virtual ObservableCollection<Message> Messages { get; set; }

        private string _draft;
        public string Draft
        {
            get
            {
                return _draft;
            }
            set
            {
                _draft = value;
                OnPropertyChanged("Draft");
            }
        }
    }
}
