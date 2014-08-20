using SharpXMPP.WPF.ViewModels;
using System.Windows;

namespace SharpXMPP.WPF.Views
{
    /// <summary>
    /// Interaction logic for NewAccount.xaml
    /// </summary>
    public partial class NewAccount : Window
    {
        public NewAccount()
        {
            InitializeComponent();
            DataContext = new NewAccountViewModel { CloseAction = () => Close() };
        }
    }
}
