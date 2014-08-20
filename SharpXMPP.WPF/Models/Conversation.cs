using SharpXMPP.WPF.ViewModels;
using System.Collections.Generic;

namespace SharpXMPP.WPF.Models
{
    public class Conversation : ViewModelBase
    {
        public int ConversationID { get; set; }
        public Account Account { get; set; }
        public User User { get; set; }
        public virtual List<Message> Messages { get; set; }

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
