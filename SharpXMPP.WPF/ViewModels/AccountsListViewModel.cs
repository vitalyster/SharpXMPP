using SharpXMPP.WPF.Helpers;
using SharpXMPP.WPF.Models;
using SharpXMPP.WPF.Views;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Windows;

namespace SharpXMPP.WPF.ViewModels
{
    public class AccountsListViewModel : ViewModelBase
    {
        public AccountsListViewModel()
        {
            Db.Accounts.Load();
            Accounts = Db.Accounts.Local;
            AddAccountCommand = new DelegateCommand<RoutedEventArgs>((e) => {
                var newaw = new NewAccount();
                newaw.ShowDialog();
            }, () => true);
            DeleteAccountCommand = new DelegateCommand<Account>((account) =>
            {
                if (account != null)
                {
                    Db.Accounts.Remove(account);
                    Db.SaveChanges();
                }
            }, () => true);
        }
        public ObservableCollection<Account> Accounts { get; set; }
        public DelegateCommand<RoutedEventArgs> AddAccountCommand { get; set; }
        public DelegateCommand<Account> DeleteAccountCommand { get; set; }
    }
}
