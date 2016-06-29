using System.ComponentModel;
using System.Data.Entity;
using System.Windows;
using System.Windows.Data;
using SharpXMPP.WPF.Helpers;
using SharpXMPP.WPF.Models;
using SharpXMPP.WPF.Views;

namespace SharpXMPP.WPF.ViewModels
{
    public class AccountsListViewModel : ViewModelBase
    {
        public AccountsListViewModel()
        {
            App.DB.Accounts.Load();
            Accounts = new CollectionViewSource { Source = App.DB.Accounts.Local }.View;
            AddAccountCommand = new DelegateCommand<RoutedEventArgs>((e) => {
                var newaw = new NewAccount();
                newaw.ShowDialog();
            }, () => true);
            DeleteAccountCommand = new DelegateCommand<Account>((account) =>
            {
                if (account != null)
                {
                    App.DB.Accounts.Remove(account);
                    App.DB.SaveChanges();
                    Accounts.Refresh();
                }
            }, () => true);
        }
        public ICollectionView Accounts { get; set; }
        public DelegateCommand<RoutedEventArgs> AddAccountCommand { get; set; }
        public DelegateCommand<Account> DeleteAccountCommand { get; set; }
    }
}
