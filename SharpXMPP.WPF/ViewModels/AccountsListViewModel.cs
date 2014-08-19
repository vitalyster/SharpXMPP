using SharpXMPP.WPF.Helpers;
using SharpXMPP.WPF.Models;
using SharpXMPP.WPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Windows;

namespace SharpXMPP.WPF.ViewModels
{
    public class AccountsListViewModel : ViewModelBase
    {
        public AccountsListViewModel()
        {
            db.Accounts.Load();
            Accounts = db.Accounts.Local;
            AddAccountCommand = new DelegateCommand<RoutedEventArgs>((e) => {
                var newaw = new NewAccount();
                newaw.ShowDialog();
            }, () => true);
            DeleteAccountCommand = new DelegateCommand<Account>((account) =>
            {
                if (account != null)
                {
                    db.Accounts.Remove(account);
                    db.SaveChanges();
                }
            }, () => true);
        }
        public ObservableCollection<Account> Accounts { get; set; }
        public DelegateCommand<RoutedEventArgs> AddAccountCommand { get; set; }
        public DelegateCommand<Account> DeleteAccountCommand { get; set; }
    }
}
