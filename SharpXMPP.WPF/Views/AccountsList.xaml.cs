using System.Windows.Controls;
using SharpXMPP.WPF.ViewModels;

namespace SharpXMPP.WPF.Views
{
    /// <summary>
    /// Interaction logic for AccountsList.xaml
    /// </summary>
    public partial class AccountsList : UserControl
    {
        public AccountsList()
        {
            InitializeComponent();
            DataContext = new AccountsListViewModel();
        }
    }
}
