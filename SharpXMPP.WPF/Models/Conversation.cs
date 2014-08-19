using SharpXMPP.WPF.ViewModels;
using SharpXMPP.XMPP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SharpXMPP.WPF.Models
{
    public class Conversation : ViewModelBase
    {
        public int ConversationID { get; set; }
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
