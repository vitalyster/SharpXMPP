using SharpXMPP.WPF.Helpers;
using SharpXMPP.WPF.Models;
using SharpXMPP.XMPP;
using System;
using System.Windows;

namespace SharpXMPP.WPF.ViewModels
{
    public class NewAccountViewModel : ViewModelBase
    {
        public Action CloseAction { get; set; }
        public bool? DialogResult { get; set; }

        public NewAccountViewModel()
        {
            SaveAccountCommand = new DelegateCommand<RoutedEventArgs>((e) =>
            {
                App.DB.Accounts.Add(new Account { JID = JID, Password = Password });
                App.DB.SaveChanges();
                CloseAction();
                DialogResult = true;
            }, () => true);
        }
        private string _jid;
        public string JID
        {
            get
            {
                return _jid;
            }
            set
            {
                _jid = value;
                OnPropertyChanged("JID");
            }
        }


        private string _password;

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }

        public DelegateCommand<RoutedEventArgs> SaveAccountCommand { get; set; }
    }
}
